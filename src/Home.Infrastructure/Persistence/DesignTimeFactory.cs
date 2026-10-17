using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Smoehring.Home.Infrastructure.Persistence;

/// <summary>Used by `dotnet ef` only.</summary>
internal sealed class DesignTimeFactory : IDesignTimeDbContextFactory<HomeDbContext>
{
    public HomeDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<HomeDbContext>()
            .UseNpgsql("Host=localhost;Database=home_design", n => n.MigrationsHistoryTable("__ef_migrations", HomeDbContext.Schema))
            .UseSnakeCaseNamingConvention().Options;
        return new HomeDbContext(options, TimeProvider.System);
    }
}
