using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Application.Abstractions;
using Smoehring.Home.Domain.Artworks;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.Common;
using Smoehring.Home.Domain.MediaLibrary;
using Smoehring.Home.Domain.Recipes;

namespace Smoehring.Home.Infrastructure.Persistence;

public class HomeDbContext(DbContextOptions<HomeDbContext> options, TimeProvider clock) : DbContext(options), IApplicationDbContext
{
    public const string Schema = "home";

    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<AssetState> AssetStates => Set<AssetState>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<AssetFile> AssetFiles => Set<AssetFile>();
    public DbSet<Media> Mediae => Set<Media>();
    public DbSet<MediaGroup> MediaGroups => Set<MediaGroup>();
    public DbSet<Artwork> Artworks => Set<Artwork>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<ArtworkSet> ArtworkSets => Set<ArtworkSet>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIteration> RecipeIterations => Set<RecipeIteration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HomeDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = clock.GetUtcNow();
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added) entry.Entity.CreatedAt = now;
            if (entry.State is EntityState.Added or EntityState.Modified) entry.Entity.UpdatedAt = now;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

public sealed class HomeDbContextFactory(IDbContextFactory<HomeDbContext> inner) : IApplicationDbContextFactory
{
    public IApplicationDbContext Create() => inner.CreateDbContext();
}
