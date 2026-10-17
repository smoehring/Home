using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Smoehring.Home.Application.Abstractions;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.MediaLibrary;

namespace Smoehring.Home.Application.Lookups;

/// <summary>Cached, async access to rarely changing lookup data.</summary>
public class LookupService(IApplicationDbContextFactory dbFactory, IMemoryCache cache)
{
    public Task<IReadOnlyList<AssetState>> AssetStatesAsync(CancellationToken ct = default) =>
        Cached("states", db => db.AssetStates.AsNoTracking().OrderBy(s => s.Id).ToListAsync(ct));

    public Task<IReadOnlyList<Currency>> CurrenciesAsync(CancellationToken ct = default) =>
        Cached("currencies", db => db.Currencies.AsNoTracking().OrderBy(s => s.Code).ToListAsync(ct));

    public Task<IReadOnlyList<Language>> LanguagesAsync(CancellationToken ct = default) =>
        Cached("languages", db => db.Languages.AsNoTracking().OrderBy(s => s.DisplayName).ToListAsync(ct));

    private async Task<IReadOnlyList<T>> Cached<T>(string key, Func<IApplicationDbContext, Task<List<T>>> load)
    {
        if (cache.TryGetValue(key, out IReadOnlyList<T>? hit) && hit is not null) return hit;
        await using var db = dbFactory.Create();
        IReadOnlyList<T> value = await load(db);
        cache.Set(key, value, TimeSpan.FromMinutes(10));
        return value;
    }
}
