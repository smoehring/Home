using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Smoehring.Home.Application.Artworks;
using Smoehring.Home.Application.Assets;
using Smoehring.Home.Application.Recipes;
using Smoehring.Home.Domain.Artworks;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Infrastructure.Persistence;

namespace Smoehring.Home.Infrastructure.Tests;

/// <summary>Runs only when HOME_TEST_POSTGRES points at a server (e.g. "Host=localhost;Username=home;Password=home").</summary>
public sealed class PostgresFactAttribute : FactAttribute
{
    public PostgresFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOME_TEST_POSTGRES"))) Skip = "HOME_TEST_POSTGRES is not set.";
    }
}

public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly string _database = "home_test_" + Guid.NewGuid().ToString("N");
    public string? ConnectionString { get; private set; }
    public IDbContextFactory<HomeDbContext>? Factory { get; private set; }

    public async Task InitializeAsync()
    {
        var server = Environment.GetEnvironmentVariable("HOME_TEST_POSTGRES");
        if (string.IsNullOrEmpty(server)) return;
        ConnectionString = $"{server};Database={_database}";
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddDbContextFactory<HomeDbContext>(o => o.UseNpgsql(ConnectionString,
            n => n.MigrationsHistoryTable("__ef_migrations", HomeDbContext.Schema)).UseSnakeCaseNamingConvention());
        services.AddSingleton(TimeProvider.System);
        Factory = services.BuildServiceProvider().GetRequiredService<IDbContextFactory<HomeDbContext>>();
        await using var db = await Factory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (Factory is null) return;
        await using var db = await Factory.CreateDbContextAsync();
        await db.Database.EnsureDeletedAsync();
    }

    public IApplicationDbContextFactoryAdapter Adapter => new(Factory!);
}

public sealed class IApplicationDbContextFactoryAdapter(IDbContextFactory<HomeDbContext> inner) : Smoehring.Home.Application.Abstractions.IApplicationDbContextFactory
{
    public Smoehring.Home.Application.Abstractions.IApplicationDbContext Create() => inner.CreateDbContext();
}

public class PostgresIntegrationTests(PostgresFixture fx) : IClassFixture<PostgresFixture>
{
    [PostgresFact]
    public async Task Migrations_apply_and_seed_data_exists()
    {
        await using var db = await fx.Factory!.CreateDbContextAsync();
        Assert.Equal(6, await db.AssetStates.CountAsync());
        Assert.Contains(await db.Currencies.ToListAsync(), c => c.Code == "EUR");
        Assert.Empty(await db.Database.GetPendingMigrationsAsync());
    }

    [PostgresFact]
    public async Task Asset_roundtrip_sets_uuid_and_audit_columns_and_search_works()
    {
        var svc = new AssetService(fx.Adapter, new NullStore(), TimeProvider.System);
        var id = await svc.SaveAsync(new SaveAssetCommand(null, "Camera", "Sony", "Camera", null, null, "SN-PG", "A7", 99.5m, 1, new DateOnly(2024, 1, 1), null));
        var found = await svc.SearchAsync(new AssetQuery("sn-pg"));
        Assert.Equal(id, Assert.Single(found.Items).Uuid);
        Assert.Equal(99.5m, (await svc.GetAsync(id))!.PurchaseAmount);
        await svc.ChangeStateAsync(id, 3, "stolen");
        Assert.Equal("Stolen", (await svc.GetAsync(id))!.State);
    }

    [PostgresFact]
    public async Task Concurrent_updates_are_detected_with_xmin()
    {
        var svc = new AssetService(fx.Adapter, new NullStore(), TimeProvider.System);
        var id = await svc.SaveAsync(new SaveAssetCommand(null, "Lens", null, null, null, null, null, null, null, null, null, null));
        await using var a = await fx.Factory!.CreateDbContextAsync();
        await using var b = await fx.Factory.CreateDbContextAsync();
        var x = await a.Assets.SingleAsync(s => s.Uuid == id);
        var y = await b.Assets.SingleAsync(s => s.Uuid == id);
        x.Name = "first"; await a.SaveChangesAsync();
        y.Name = "second";
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => b.SaveChangesAsync());
    }

    [PostgresFact]
    public async Task Only_one_primary_artwork_version_is_allowed_by_the_database()
    {
        var svc = new ArtworkService(fx.Adapter, new NullStore(), TimeProvider.System);
        var uuid = await svc.CreateAsync(new CreateArtworkCommand("Art", false, ["Alice"], []));
        await svc.AddVersionAsync(uuid, "one", true, null);
        await svc.AddVersionAsync(uuid, "two", true, null);
        await using var db = await fx.Factory!.CreateDbContextAsync();
        var versions = await db.Artworks.Where(a => a.Uuid == uuid).SelectMany(a => a.Versions).ToListAsync();
        Assert.Equal("two", Assert.Single(versions, v => v.IsPrimary).Label);
        db.Set<ArtworkVersion>().Add(new ArtworkVersion { ArtworkId = versions[0].ArtworkId, Label = "dup", IsPrimary = true });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [PostgresFact]
    public async Task Recipe_steps_and_iterations_persist_with_dependencies()
    {
        var svc = new RecipeService(fx.Adapter, TimeProvider.System);
        var id = await svc.CreateAsync("Bread", 4, null);
        await svc.SetStepsAsync(id, [new("Mix", TimeSpan.FromMinutes(10), true, null, null, []), new("Bake", TimeSpan.FromMinutes(40), false, null, null, [0])]);
        await svc.SetStepsAsync(id, [new("Mix", TimeSpan.FromMinutes(10), true, null, null, []), new("Bake", TimeSpan.FromMinutes(45), false, null, null, [0])]);
        await svc.NewIterationAsync(id, "v2");
        var plan = (await svc.PlanAsync(id, null))!;
        Assert.Equal(2, plan.Number);
        Assert.Equal(TimeSpan.FromMinutes(55), plan.Total);
        Assert.Equal(TimeSpan.FromMinutes(55), (await svc.PlanAsync(id, 1))!.Total);
    }

    private sealed class NullStore : Smoehring.Home.Application.Abstractions.IFileStore
    {
        public Task<Smoehring.Home.Application.Abstractions.StoredFile> SaveAsync(string k, Stream c, CancellationToken ct = default) => Task.FromResult(new Smoehring.Home.Application.Abstractions.StoredFile(k, 0, ""));
        public Task<Stream?> OpenReadAsync(string k, CancellationToken ct = default) => Task.FromResult<Stream?>(null);
        public Task DeleteAsync(string k, CancellationToken ct = default) => Task.CompletedTask;
    }
}
