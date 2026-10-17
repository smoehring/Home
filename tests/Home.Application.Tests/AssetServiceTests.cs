using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Application.Assets;
using Smoehring.Home.Application.Common;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Application.Tests;

public class AssetServiceTests
{
    private readonly TestDb _db = new();
    private readonly InMemoryFileStore _files = new();
    private AssetService Sut => new(_db, _files, _db.Clock);

    private static SaveAssetCommand Cmd(string name = "Camera", string? serial = "SN-1", Guid? uuid = null) =>
        new(uuid, name, "Sony", "Camera", "Shelf", null, serial, "A7", 1299.99m, 1, new DateOnly(2024, 5, 1), "Shop");

    [Fact]
    public async Task Save_creates_asset_with_device_purchase_brand_and_type()
    {
        var id = await Sut.SaveAsync(Cmd());
        var details = await Sut.GetAsync(id);
        Assert.NotNull(details);
        Assert.Equal("SN-1", details.SerialNumber);
        Assert.Equal(1299.99m, details.PurchaseAmount);
        Assert.Equal("EUR", details.PurchaseCurrency);
        Assert.Equal("Sony", details.Brand);
        Assert.Equal(new DateOnly(2024, 5, 1), details.PurchaseDate);
    }

    [Fact]
    public async Task Save_reuses_existing_brand_ignoring_case()
    {
        await Sut.SaveAsync(Cmd("A"));
        await Sut.SaveAsync(Cmd("B") with { Brand = "SONY" });
        await using var raw = _db.Raw();
        Assert.Equal(1, await raw.Brands.CountAsync());
    }

    [Fact]
    public async Task Save_updates_an_existing_asset()
    {
        var id = await Sut.SaveAsync(Cmd());
        await Sut.SaveAsync(Cmd("Renamed", "SN-2", id));
        var details = await Sut.GetAsync(id);
        Assert.Equal("Renamed", details!.Name);
        Assert.Equal("SN-2", details.SerialNumber);
        await using var raw = _db.Raw();
        Assert.Equal(1, await raw.Assets.CountAsync());
    }

    [Fact]
    public async Task Save_without_serial_and_model_removes_device() =>
        Assert.Null((await Sut.GetAsync(await Sut.SaveAsync(Cmd(serial: null) with { ModelNumber = null })))!.SerialNumber);

    [Fact]
    public async Task Save_rejects_blank_name() =>
        await Assert.ThrowsAsync<DomainException>(() => Sut.SaveAsync(Cmd(" ")));

    [Fact]
    public async Task Save_rejects_negative_purchase_amount() =>
        await Assert.ThrowsAsync<DomainException>(() => Sut.SaveAsync(Cmd() with { PurchaseAmount = -1 }));

    [Fact]
    public async Task Save_of_unknown_asset_fails() =>
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.SaveAsync(Cmd(uuid: Guid.NewGuid())));

    [Fact]
    public async Task Search_filters_by_name_brand_and_serial_and_pages()
    {
        await Sut.SaveAsync(Cmd("Camera", "ABC-123"));
        await Sut.SaveAsync(Cmd("Lens", "XYZ-9") with { Brand = "Canon" });
        await Sut.SaveAsync(Cmd("Tripod", null) with { ModelNumber = null });

        Assert.Equal(3, (await Sut.SearchAsync(new AssetQuery())).TotalCount);
        Assert.Equal("Camera", Assert.Single((await Sut.SearchAsync(new AssetQuery("abc"))).Items).Name);
        Assert.Equal("Lens", Assert.Single((await Sut.SearchAsync(new AssetQuery("canon"))).Items).Name);
        Assert.Equal("Tripod", Assert.Single((await Sut.SearchAsync(new AssetQuery("trip"))).Items).Name);
        var page2 = await Sut.SearchAsync(new AssetQuery(null, 2, 2));
        Assert.Single(page2.Items);
        Assert.Equal(3, page2.TotalCount);
    }

    [Fact]
    public async Task Change_state_records_history_with_the_current_time()
    {
        var id = await Sut.SaveAsync(Cmd());
        await Sut.ChangeStateAsync(id, 3, "stolen");
        Assert.Equal("Stolen", (await Sut.GetAsync(id))!.State);
        await using var raw = _db.Raw();
        var change = await raw.Assets.SelectMany(a => a.StateChanges).SingleAsync();
        Assert.Equal(_db.Clock.Now, change.At);
        Assert.Equal("stolen", change.Note);
    }

    [Fact]
    public async Task Change_state_to_unknown_state_fails()
    {
        var id = await Sut.SaveAsync(Cmd());
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.ChangeStateAsync(id, 999, null));
    }

    [Fact]
    public async Task Delete_is_soft_and_hides_the_asset()
    {
        var id = await Sut.SaveAsync(Cmd());
        await Sut.DeleteAsync(id);
        Assert.Null(await Sut.GetAsync(id));
        Assert.Empty((await Sut.SearchAsync(new AssetQuery())).Items);
        await using var raw = _db.Raw();
        Assert.Equal(1, await raw.Assets.CountAsync());
    }

    [Fact]
    public async Task Add_file_stores_content_and_metadata()
    {
        var id = await Sut.SaveAsync(Cmd());
        var info = await Sut.AddFileAsync(id, "..\\..\\invoice.pdf", "application/pdf", FileKind.Invoice, new MemoryStream([1, 2, 3]));
        Assert.Equal("invoice.pdf", info.FileName);
        Assert.Equal(3, info.Size);
        var key = Assert.Single(_files.Files).Key;
        Assert.StartsWith(id.ToString("N"), key);
        Assert.EndsWith(".pdf", key);
        Assert.Single((await Sut.GetAsync(id))!.Files);
    }

    [Fact]
    public async Task Open_file_returns_content_or_null()
    {
        var id = await Sut.SaveAsync(Cmd());
        var info = await Sut.AddFileAsync(id, "a.txt", "text/plain", FileKind.Other, new MemoryStream([7]));
        var opened = await Sut.OpenFileAsync(info.Uuid);
        Assert.NotNull(opened);
        Assert.Equal(1, opened.Value.Content.Length);
        Assert.Null(await Sut.OpenFileAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Missing_required_serial_is_flagged()
    {
        await using (var raw = _db.Raw())
        {
            raw.AssetTypes.Add(new AssetType { Name = "Camera", RequiresSerialNumber = true });
            await raw.SaveChangesAsync();
        }
        var id = await Sut.SaveAsync(Cmd(serial: null) with { ModelNumber = null });
        Assert.True((await Sut.GetAsync(id))!.MissingRequiredSerialNumber);
    }

    [Fact]
    public async Task Insurance_report_contains_only_owned_assets()
    {
        var owned = await Sut.SaveAsync(Cmd("Owned"));
        var stolen = await Sut.SaveAsync(Cmd("Stolen item"));
        await Sut.ChangeStateAsync(stolen, 3, null);
        var rows = await Sut.InsuranceReportAsync();
        var row = Assert.Single(rows);
        Assert.Equal("Owned", row.Name);
        Assert.Equal(1299.99m, row.CurrentValue);
        Assert.NotEqual(Guid.Empty, owned);
    }

    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("a,b", "\"a,b\"")]
    [InlineData("say \"hi\"", "\"say \"\"hi\"\"\"")]
    [InlineData("=SUM(A1)", "'=SUM(A1)")]
    [InlineData("@cmd", "'@cmd")]
    [InlineData(null, "")]
    public void Csv_escapes_fields_and_neutralises_formulas(string? input, string expected) =>
        Assert.Equal(expected, AssetService.Csv(input));

    [Fact]
    public void Csv_has_header_and_invariant_numbers()
    {
        var csv = AssetService.ToCsv([new InsuranceReportRow("Cam", "Sony", "SN", "M", new DateOnly(2024, 5, 1), 1299.5m, "EUR", 1000m, "Available")]);
        var lines = csv.TrimEnd('\n').Split('\n');
        Assert.StartsWith("Name,Brand", lines[0]);
        Assert.Equal("Cam,Sony,SN,M,2024-05-01,1299.5,EUR,1000,Available", lines[1]);
    }
}
