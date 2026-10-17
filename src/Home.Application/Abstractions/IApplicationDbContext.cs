using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Domain.Artworks;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.MediaLibrary;
using Smoehring.Home.Domain.Recipes;

namespace Smoehring.Home.Application.Abstractions;

/// <summary>Persistence port. Implemented in Infrastructure.</summary>
public interface IApplicationDbContext : IAsyncDisposable
{
    DbSet<Asset> Assets { get; }
    DbSet<AssetType> AssetTypes { get; }
    DbSet<AssetState> AssetStates { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Currency> Currencies { get; }
    DbSet<Language> Languages { get; }
    DbSet<AssetFile> AssetFiles { get; }
    DbSet<Media> Mediae { get; }
    DbSet<MediaGroup> MediaGroups { get; }
    DbSet<Artwork> Artworks { get; }
    DbSet<Artist> Artists { get; }
    DbSet<Character> Characters { get; }
    DbSet<ArtworkSet> ArtworkSets { get; }
    DbSet<Recipe> Recipes { get; }
    DbSet<RecipeIteration> RecipeIterations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>Creates short-lived contexts (required for Blazor Server, which must not share a context per circuit).</summary>
public interface IApplicationDbContextFactory
{
    IApplicationDbContext Create();
}
