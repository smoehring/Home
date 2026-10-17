using Smoehring.Home.Application.Assets;
using Smoehring.Home.Application.Common;
using Smoehring.Home.Application.MediaLibrary;
using Smoehring.Home.Domain.Common;
using Smoehring.Home.Domain.MediaLibrary;

namespace Smoehring.Home.Application.Tests;

public class MediaServiceTests
{
    private readonly TestDb _db = new();
    private MediaService Sut => new(_db, _db.Clock);

    private static SaveMediaCommand Cmd(string title = "Portal", MediaKind kind = MediaKind.Game, string? group = "Portal Series", int? order = 1,
        IReadOnlyList<MediaNameItem>? names = null, IReadOnlyList<MediaExternalIdItem>? ids = null, Guid? uuid = null) =>
        new(uuid, title, kind, group, order, new DateOnly(2007, 10, 10), names ?? [], ids ?? []);

    [Fact]
    public async Task Save_creates_an_inventory_asset_with_media_details()
    {
        var id = await Sut.SaveAsync(Cmd(names: [new("Portal DE", 1)], ids: [new("Steam", "400")]));
        var m = await Sut.GetAsync(id);
        Assert.NotNull(m);
        Assert.Equal("Portal", m.Title);
        Assert.Equal("Portal Series", m.Group);
        Assert.Equal("Portal DE", Assert.Single(m.Names).Name);
        Assert.Equal("400", Assert.Single(m.ExternalIds).Value);
        var asset = await new AssetService(_db, new InMemoryFileStore(), _db.Clock).GetAsync(id);
        Assert.NotNull(asset);
    }

    [Fact]
    public async Task Save_updates_and_replaces_names_and_ids_and_reuses_groups()
    {
        var id = await Sut.SaveAsync(Cmd(names: [new("A", 1), new("B", 2)], ids: [new("Steam", "1")]));
        await Sut.SaveAsync(Cmd("Portal 1", names: [new("C", 2)], ids: [], uuid: id));
        await Sut.SaveAsync(Cmd("Portal 2", order: 2));
        var m = await Sut.GetAsync(id);
        Assert.Equal("Portal 1", m!.Title);
        Assert.Equal("C", Assert.Single(m.Names).Name);
        Assert.Empty(m.ExternalIds);
        Assert.Equal(1, _db.Raw().MediaGroups.Count());
    }

    [Fact]
    public async Task Save_drops_blank_rows_and_duplicates()
    {
        var id = await Sut.SaveAsync(Cmd(names: [new(" X ", 1), new("X", 1), new("", 1)], ids: [new("", ""), new("A", "1"), new("A ", "1")]));
        var m = await Sut.GetAsync(id);
        Assert.Single(m!.Names);
        Assert.Single(m.ExternalIds);
    }

    [Fact]
    public async Task Save_validates_input()
    {
        await Assert.ThrowsAsync<DomainException>(() => Sut.SaveAsync(Cmd(title: " ")));
        await Assert.ThrowsAsync<DomainException>(() => Sut.SaveAsync(Cmd(order: -1)));
        await Assert.ThrowsAsync<DomainException>(() => Sut.SaveAsync(Cmd(ids: [new("Steam", "")])));
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.SaveAsync(Cmd(names: [new("X", 9999)])));
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.SaveAsync(Cmd(uuid: Guid.NewGuid())));
    }

    [Fact]
    public async Task Plain_assets_are_not_media_items()
    {
        var plain = await new AssetService(_db, new InMemoryFileStore(), _db.Clock)
            .SaveAsync(new SaveAssetCommand(null, "Camera", null, null, null, null, null, null, null, null, null, null));
        Assert.Null(await Sut.GetAsync(plain));
        Assert.Empty((await Sut.SearchAsync(new MediaQuery())).Items);
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.SaveAsync(Cmd(uuid: plain)));
    }

    [Fact]
    public async Task Search_filters_by_text_alternative_name_external_id_and_kind()
    {
        await Sut.SaveAsync(Cmd("Portal", names: [new("Portal Deutsch", 1)], ids: [new("Steam", "400")]));
        await Sut.SaveAsync(Cmd("Alien", MediaKind.Film, group: null, order: null, ids: [new("IMDb", "tt0078748")]));
        Assert.Equal(2, (await Sut.SearchAsync(new MediaQuery())).TotalCount);
        Assert.Equal("Portal", Assert.Single((await Sut.SearchAsync(new MediaQuery("deutsch"))).Items).Title);
        Assert.Equal("Alien", Assert.Single((await Sut.SearchAsync(new MediaQuery("TT0078"))).Items).Title);
        Assert.Equal("Alien", Assert.Single((await Sut.SearchAsync(new MediaQuery(Kind: MediaKind.Film))).Items).Title);
    }

    [Fact]
    public async Task Search_pages_results()
    {
        for (var i = 0; i < 5; i++) await Sut.SaveAsync(Cmd($"M{i}", group: null, order: null));
        var page2 = await Sut.SearchAsync(new MediaQuery(Page: 2, PageSize: 2));
        Assert.Equal(5, page2.TotalCount);
        Assert.Equal(["M2", "M3"], page2.Items.Select(i => i.Title));
    }

    [Fact]
    public async Task Delete_hides_the_item()
    {
        var id = await Sut.SaveAsync(Cmd());
        await Sut.DeleteAsync(id);
        Assert.Null(await Sut.GetAsync(id));
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.DeleteAsync(id));
    }

    [Fact]
    public async Task Languages_are_seeded()
    {
        Assert.NotEmpty(await Sut.LanguagesAsync());
    }
}
