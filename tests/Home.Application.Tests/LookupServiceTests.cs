using Microsoft.Extensions.Caching.Memory;
using Smoehring.Home.Application.Lookups;
using Smoehring.Home.Domain.Assets;

namespace Smoehring.Home.Application.Tests;

public class LookupServiceTests
{
    [Fact]
    public async Task Seeded_lookups_are_returned_and_cached()
    {
        var db = new TestDb();
        var sut = new LookupService(db, new MemoryCache(new MemoryCacheOptions()));
        Assert.Equal(6, (await sut.AssetStatesAsync()).Count);
        Assert.Contains(await sut.CurrenciesAsync(), c => c.Code == "EUR");
        Assert.Contains(await sut.LanguagesAsync(), l => l.CultureName == "de-DE");

        await using (var raw = db.Raw())
        {
            raw.Currencies.Add(new Currency { Code = "CHF", Name = "Franc", Symbol = "Fr" });
            await raw.SaveChangesAsync();
        }
        Assert.DoesNotContain(await sut.CurrenciesAsync(), c => c.Code == "CHF");
    }

    [Fact]
    public async Task Default_state_is_available()
    {
        var states = await new LookupService(new TestDb(), new MemoryCache(new MemoryCacheOptions())).AssetStatesAsync();
        Assert.Equal("Available", states.Single(s => s.IsDefault).Name);
    }
}
