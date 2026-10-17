using Smoehring.Home.Application.Assets;
using Smoehring.Home.Application.Common;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Application.Tests;

public class ValuationAndLabelTests
{
    private readonly TestDb _db = new();
    private AssetService Sut => new(_db, new InMemoryFileStore(), _db.Clock);

    private Task<Guid> Create(string name, string? serial = null) =>
        Sut.SaveAsync(new SaveAssetCommand(null, name, "Sony", "Camera", null, null, serial, null, null, null, null, null));

    [Fact]
    public async Task Valuations_are_listed_newest_first()
    {
        var id = await Create("Camera");
        await Sut.AddValuationAsync(new AddValuationCommand(id, 900, 1, new DateOnly(2024, 1, 1), ValuationReason.Appraisal));
        await Sut.AddValuationAsync(new AddValuationCommand(id, 1100, 1, new DateOnly(2025, 1, 1), ValuationReason.ReplacementValue));
        var list = await Sut.ValuationsAsync(id);
        Assert.Equal([1100m, 900m], list.Select(v => v.Amount));
        Assert.Equal("EUR", list[0].Currency);
    }

    [Fact]
    public async Task Latest_valuation_drives_the_insurance_report()
    {
        var id = await Create("Camera");
        await Sut.AddValuationAsync(new AddValuationCommand(id, 900, 1, new DateOnly(2024, 1, 1), ValuationReason.Appraisal));
        await Sut.AddValuationAsync(new AddValuationCommand(id, 1100, 1, new DateOnly(2025, 1, 1), ValuationReason.Appraisal));
        var row = Assert.Single(await Sut.InsuranceReportAsync());
        Assert.Equal(1100m, row.CurrentValue);
    }

    [Fact]
    public async Task Negative_valuation_unknown_currency_and_unknown_asset_are_rejected()
    {
        var id = await Create("Camera");
        await Assert.ThrowsAsync<DomainException>(() => Sut.AddValuationAsync(new AddValuationCommand(id, -1, 1, new DateOnly(2024, 1, 1), ValuationReason.Appraisal)));
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.AddValuationAsync(new AddValuationCommand(id, 1, 999, new DateOnly(2024, 1, 1), ValuationReason.Appraisal)));
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.AddValuationAsync(new AddValuationCommand(Guid.NewGuid(), 1, 1, new DateOnly(2024, 1, 1), ValuationReason.Appraisal)));
    }

    [Fact]
    public async Task Valuation_can_be_removed_but_not_through_another_asset()
    {
        var a = await Create("A");
        var b = await Create("B");
        await Sut.AddValuationAsync(new AddValuationCommand(a, 10, 1, new DateOnly(2024, 1, 1), ValuationReason.Purchase));
        var valuationId = (await Sut.ValuationsAsync(a)).Single().Id;
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.RemoveValuationAsync(b, valuationId));
        await Sut.RemoveValuationAsync(a, valuationId);
        Assert.Empty(await Sut.ValuationsAsync(a));
    }

    [Fact]
    public async Task Labels_can_be_filtered_by_search_and_printed_state()
    {
        var a = await Create("Alpha", "SER-111");
        await Create("Beta", "SER-222");
        Assert.Equal(2, (await Sut.LabelsAsync(new LabelQuery())).Count);
        Assert.Equal("Alpha", Assert.Single(await Sut.LabelsAsync(new LabelQuery("ser-111"))).Name);

        await Sut.MarkPrintedAsync([a]);
        var unprinted = await Sut.LabelsAsync(new LabelQuery(OnlyUnprinted: true));
        Assert.Equal("Beta", Assert.Single(unprinted).Name);
        Assert.Equal(_db.Clock.Now, (await Sut.LabelsAsync(new LabelQuery("alpha"))).Single().PrintedAt);
    }

    [Fact]
    public async Task Deleted_assets_get_no_label_and_empty_mark_is_a_noop()
    {
        var a = await Create("Alpha");
        await Sut.DeleteAsync(a);
        Assert.Empty(await Sut.LabelsAsync(new LabelQuery()));
        await Sut.MarkPrintedAsync([]);
    }
}
