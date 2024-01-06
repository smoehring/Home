using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using Serilog;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Ui.BlazorSrv.Data;
using Microsoft.Extensions.Azure;
using Smoehring.Home.Ui.BlazorSrv;

Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

Log.Information("Application starting");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, provider, logger) =>
{
    logger
        .ReadFrom.Services(provider)
        .ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddDbContextFactory<DatabaseContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), contextOptionsBuilder => contextOptionsBuilder.CommandTimeout(60 * 10));

    if (!builder.Environment.IsDevelopment()) return;
    optionsBuilder.EnableDetailedErrors();
    optionsBuilder.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<UserCacheService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DatabaseContext>("Sql Server", HealthStatus.Unhealthy);

builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddMudServices();
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["ConnectionStrings:AzureStorage"], preferMsi: false);
    clientBuilder.AddQueueServiceClient(builder.Configuration["ConnectionStrings:AzureStorage"], preferMsi: false);
});

var app = builder.Build();

app.MigrateDatabase<DatabaseContext>();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/health", new HealthCheckOptions()
{
    AllowCachingResponses = false,
    ResultStatusCodes = new Dictionary<HealthStatus, int>()
    {
        { HealthStatus.Healthy, StatusCodes.Status200OK },
        { HealthStatus.Degraded, 210 },
        { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable }
    }
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
