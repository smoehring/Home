using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smoehring.Home.Application.Abstractions;
using Smoehring.Home.Infrastructure.Persistence;
using Smoehring.Home.Infrastructure.Storage;

namespace Smoehring.Home.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, bool detailedErrors = false)
    {
        var connection = config.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured.");
        services.AddSingleton(TimeProvider.System);
        services.AddDbContextFactory<HomeDbContext>(o =>
        {
            o.UseNpgsql(connection, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations", HomeDbContext.Schema);
                npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }).UseSnakeCaseNamingConvention();
            if (detailedErrors) { o.EnableDetailedErrors(); o.EnableSensitiveDataLogging(); }
        });
        services.AddSingleton<IApplicationDbContextFactory, HomeDbContextFactory>();
        services.Configure<StorageOptions>(config.GetSection(StorageOptions.Section));
        services.AddSingleton<IFileStore, LocalFileStore>();
        return services;
    }
}
