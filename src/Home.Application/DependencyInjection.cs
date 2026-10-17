using Microsoft.Extensions.DependencyInjection;
using Smoehring.Home.Application.Artworks;
using Smoehring.Home.Application.Assets;
using Smoehring.Home.Application.Lookups;
using Smoehring.Home.Application.MediaLibrary;
using Smoehring.Home.Application.Recipes;

namespace Smoehring.Home.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton(TimeProvider.System);
        services.AddTransient<AssetService>();
        services.AddTransient<ArtworkService>();
        services.AddTransient<RecipeService>();
        services.AddTransient<LookupService>();
        services.AddTransient<MediaService>();
        return services;
    }
}
