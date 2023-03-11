using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Smoehring.Home.Data.SqlDatabase;

Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

Log.Information("Application starting");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, provider, logger) =>
{
    logger
        .WriteTo.Console()
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithEnvironmentUserName()
        .Enrich.WithMachineName()
        .ReadFrom.Services(provider)
        .ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddDbContextFactory<DatabaseContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));

    if (!builder.Environment.IsDevelopment()) return;
    optionsBuilder.EnableDetailedErrors();
    optionsBuilder.EnableSensitiveDataLogging();
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DatabaseContext>("Sql Server", HealthStatus.Unhealthy);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

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
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
