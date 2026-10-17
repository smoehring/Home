using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Smoehring.Home.Application;
using Smoehring.Home.Infrastructure;
using Smoehring.Home.Infrastructure.Persistence;
using Smoehring.Home.Web.Components;
using Smoehring.Home.Web.Endpoints;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, logger) => logger
    .ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext().WriteTo.Console());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());

// Authentication: Azure Entra ID is the only Azure dependency. Anonymous access is only possible in Development.
var entraConfigured = !string.IsNullOrWhiteSpace(builder.Configuration["AzureAd:ClientId"]);
if (entraConfigured)
{
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
    builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
    builder.Services.AddAuthorizationBuilder().SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
}
else if (builder.Environment.IsDevelopment())
{
    Log.Warning("AzureAd is not configured - running without authentication (Development only).");
    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();
}
else throw new InvalidOperationException("AzureAd:ClientId must be configured outside Development.");
builder.Services.AddCascadingAuthenticationState();

var keysPath = builder.Configuration["DataProtection:KeysPath"];
if (!string.IsNullOrWhiteSpace(keysPath))
    builder.Services.AddDataProtection().SetApplicationName("Home").PersistKeysToFileSystem(new DirectoryInfo(keysPath));

builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    o.KnownIPNetworks.Clear();
    o.KnownProxies.Clear();
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<HomeDbContext>("postgres", HealthStatus.Unhealthy, ["ready"]);

var otlp = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
var otel = builder.Services.AddOpenTelemetry();
otel.WithTracing(t => { t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation(); if (otlp is not null) t.AddOtlpExporter(); });
otel.WithMetrics(m => { m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation(); if (otlp is not null) m.AddOtlpExporter(); });

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Configuration.GetValue<bool>("Database:AutoMigrate"))
{
    // Production should run migrations as a separate job (see deploy/k8s); this is for development convenience.
    using var scope = app.Services.CreateScope();
    await using var db = await scope.ServiceProvider.GetRequiredService<Microsoft.EntityFrameworkCore.IDbContextFactory<HomeDbContext>>().CreateDbContextAsync();
    await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync(db.Database);
}

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error", createScopeForErrors: true);
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false, AllowCachingResponses = false }).AllowAnonymous();
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = c => c.Tags.Contains("ready"), AllowCachingResponses = false }).AllowAnonymous();

app.MapStaticAssets();
app.MapHomeEndpoints();
if (entraConfigured) app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await app.RunAsync();

public partial class Program;
