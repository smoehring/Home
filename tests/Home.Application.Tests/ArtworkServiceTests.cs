using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Application.Artworks;
using Smoehring.Home.Application.Common;
using Smoehring.Home.Domain.Artworks;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Application.Tests;

public class ArtworkServiceTests
{
    private readonly TestDb _db = new();
    private readonly InMemoryFileStore _files = new();
    private ArtworkService Sut => new(_db, _files, _db.Clock);

    private Task<Guid> Create(string title = "Dragon", bool adult = false, params string[] characters) =>
        Sut.CreateAsync(new CreateArtworkCommand(title, adult, characters, []));

    [Fact]
    public async Task Create_also_creates_the_inventory_asset_and_initial_stage_history()
    {
        await Create();
        await using var raw = _db.Raw();
        var artwork = await raw.Artworks.Include(a => a.Asset).Include(a => a.StageHistory).SingleAsync();
        Assert.Equal("Dragon", artwork.Asset!.Name);
        Assert.Equal(ArtworkStage.Backlog, artwork.Stage);
        Assert.Single(artwork.StageHistory);
    }

    [Fact]
    public async Task Create_reuses_characters_and_deduplicates_names()
    {
        await Create("A", false, "Alice", "alice", " ");
        await Create("B", false, "ALICE", "Bob");
        await using var raw = _db.Raw();
        Assert.Equal(2, await raw.Characters.CountAsync());
        Assert.Equal(1, await raw.Artworks.Where(a => a.Title == "A").SelectMany(a => a.Characters).CountAsync());
    }

    [Fact]
    public async Task Create_requires_a_title() =>
        await Assert.ThrowsAsync<DomainException>(() => Create(" "));

    [Fact]
    public async Task Adult_artworks_are_hidden_unless_requested()
    {
        await Create("Safe");
        await Create("Adult", true);
        Assert.Equal("Safe", Assert.Single((await Sut.ListAsync(new ArtworkFilter())).Items).Title);
        Assert.Equal(2, (await Sut.ListAsync(new ArtworkFilter(IncludeAdult: true))).TotalCount);
    }

    [Fact]
    public async Task List_filters_by_stage_and_character_search()
    {
        var id = await Create("One", false, "Alice");
        await Create("Two", false, "Bob");
        await Sut.ChangeStageAsync(id, ArtworkStage.Sketch, null);
        Assert.Equal("One", Assert.Single((await Sut.ListAsync(new ArtworkFilter(Stage: ArtworkStage.Sketch))).Items).Title);
        Assert.Equal("Two", Assert.Single((await Sut.ListAsync(new ArtworkFilter("bob"))).Items).Title);
    }

    [Fact]
    public async Task Stage_change_is_persisted_with_history()
    {
        var id = await Create();
        await Sut.ChangeStageAsync(id, ArtworkStage.Lineart, "lines done");
        await using var raw = _db.Raw();
        var artwork = await raw.Artworks.Include(a => a.StageHistory).SingleAsync();
        Assert.Equal(ArtworkStage.Lineart, artwork.Stage);
        Assert.Equal(2, artwork.StageHistory.Count);
    }

    [Fact]
    public async Task Completing_without_a_file_is_rejected()
    {
        var id = await Create();
        await Sut.AddVersionAsync(id, "original", true, null);
        await Assert.ThrowsAsync<DomainException>(() => Sut.ChangeStageAsync(id, ArtworkStage.Complete, null));
    }

    [Fact]
    public async Task Versions_keep_a_single_primary()
    {
        var id = await Create();
        await Sut.AddVersionAsync(id, "original", true, null);
        await Sut.AddVersionAsync(id, "alt", true, null);
        await using var raw = _db.Raw();
        var versions = await raw.Artworks.SelectMany(a => a.Versions).ToListAsync();
        Assert.Equal("alt", Assert.Single(versions, v => v.IsPrimary).Label);
    }

    [Fact]
    public async Task Payments_are_summed_per_currency_with_refunds_subtracted()
    {
        var id = await Create();
        var today = new DateOnly(2026, 1, 1);
        await Sut.AddPaymentAsync(new AddPaymentCommand(id, 100, 1, today, PaymentKind.Final, PaymentMethod.PayPal, null, null));
        await Sut.AddPaymentAsync(new AddPaymentCommand(id, 30, 1, today, PaymentKind.Refund, PaymentMethod.PayPal, null, null));
        await Sut.AddPaymentAsync(new AddPaymentCommand(id, 10, 2, today, PaymentKind.Tip, PaymentMethod.Cash, null, null));
        var totals = await Sut.TotalPaidAsync(id);
        Assert.Equal(70, totals["EUR"]);
        Assert.Equal(10, totals["USD"]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Payments_must_be_positive(decimal amount)
    {
        var id = await Create();
        await Assert.ThrowsAsync<DomainException>(() =>
            Sut.AddPaymentAsync(new AddPaymentCommand(id, amount, 1, new DateOnly(2026, 1, 1), PaymentKind.Final, PaymentMethod.Other, null, null)));
    }

    [Fact]
    public async Task Unknown_artwork_is_reported() =>
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.ChangeStageAsync(Guid.NewGuid(), ArtworkStage.Sketch, null));

    [Fact]
    public async Task Artists_can_be_created_with_profile_and_are_credited_on_artworks()
    {
        var artistId = await Sut.CreateArtistAsync("Painter", "https://example.com/painter");
        var uuid = await Sut.CreateAsync(new CreateArtworkCommand("Piece", false, [], [(artistId, ArtistRole.Commission)]));
        var artist = Assert.Single(await Sut.ArtistsAsync());
        Assert.Equal("Painter", artist.DisplayName);
        Assert.Equal("example.com", artist.Profiles.Single().Type);
        Assert.Equal("Painter", Assert.Single((await Sut.ListAsync(new ArtworkFilter())).Items.Single(a => a.Uuid == uuid).Artists));
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("not a url")]
    [InlineData("ftp://example.com")]
    public async Task Artist_profile_must_be_an_http_url(string url) =>
        await Assert.ThrowsAsync<DomainException>(() => Sut.CreateArtistAsync("X", url));

    [Fact]
    public async Task Sets_track_items_and_completion()
    {
        var setId = await Sut.CreateSetAsync("Summer", 4);
        var a = await Create("A");
        var b = await Create("B");
        await Sut.AddToSetAsync(setId, a, 1);
        await Sut.AddToSetAsync(setId, a, 1);
        await Sut.AddToSetAsync(setId, b, 2);
        var set = Assert.Single(await Sut.SetsAsync());
        Assert.Equal(2, set.Items.Count);
        Assert.Equal(50m, set.CompletionPercent);
    }

    [Fact]
    public async Task Version_upload_stores_the_file_on_the_artworks_asset_and_links_it()
    {
        var id = await Create();
        await Sut.AddVersionWithFileAsync(id, "Final", true, @"C:\temp\final.png", "image/png", new MemoryStream([1, 2, 3]));
        var versions = await Sut.VersionsAsync(id);
        var v = Assert.Single(versions);
        Assert.True(v.IsPrimary);
        Assert.Equal("final.png", v.FileName);
        Assert.NotNull(v.FileUuid);
        Assert.Single(_files.Files);
        await using var raw = _db.Raw();
        var artwork = await raw.Artworks.SingleAsync();
        Assert.Equal(artwork.AssetId, raw.AssetFiles.Single().AssetId);
    }

    [Fact]
    public async Task Uploaded_primary_version_allows_completing_the_artwork()
    {
        var id = await Create();
        await Sut.AddVersionWithFileAsync(id, "Final", true, "a.png", null, new MemoryStream([1]));
        await Sut.ChangeStageAsync(id, ArtworkStage.Complete, null);
    }

    [Fact]
    public async Task Version_upload_rejects_bad_input_without_storing_anything()
    {
        var id = await Create();
        await Assert.ThrowsAsync<DomainException>(() => Sut.AddVersionWithFileAsync(id, " ", false, "a.png", null, new MemoryStream([1])));
        await Assert.ThrowsAsync<DomainException>(() => Sut.AddVersionWithFileAsync(id, "x", false, "", null, new MemoryStream([1])));
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.AddVersionWithFileAsync(Guid.NewGuid(), "x", false, "a.png", null, new MemoryStream([1])));
        Assert.Empty(_files.Files);
    }

    [Fact]
    public async Task Versions_without_file_are_listed_without_file_info_and_primary_first()
    {
        var id = await Create();
        await Sut.AddVersionAsync(id, "Alt", false, null);
        await Sut.AddVersionAsync(id, "Main", true, null);
        var list = await Sut.VersionsAsync(id);
        Assert.Equal(["Main", "Alt"], list.Select(v => v.Label));
        Assert.All(list, v => Assert.Null(v.FileUuid));
    }
}
