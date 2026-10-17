using Smoehring.Home.Domain.Assets;

namespace Smoehring.Home.Domain.Tests;

public class AssetTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void New_asset_is_available_by_default() => Assert.Equal(AssetState.AvailableId, new Asset().AssetStateId);

    [Fact]
    public void Changing_state_records_history()
    {
        var asset = new Asset();
        asset.ChangeState(3, Now, "theft report 123");
        var change = Assert.Single(asset.StateChanges);
        Assert.Equal((1, 3), (change.FromStateId, change.ToStateId));
        Assert.Equal("theft report 123", change.Note);
        Assert.Equal(3, asset.AssetStateId);
    }

    [Fact]
    public void Changing_to_the_same_state_is_ignored()
    {
        var asset = new Asset();
        asset.ChangeState(AssetState.AvailableId, Now);
        Assert.Empty(asset.StateChanges);
    }

    [Theory]
    [InlineData(true, null, true)]
    [InlineData(true, "  ", true)]
    [InlineData(true, "SN1", false)]
    [InlineData(false, null, false)]
    public void Missing_serial_number_is_detected_only_for_types_that_require_it(bool required, string? serial, bool expected)
    {
        var asset = new Asset { AssetType = new AssetType { RequiresSerialNumber = required }, Device = new Device { SerialNumber = serial } };
        Assert.Equal(expected, asset.IsMissingRequiredSerialNumber);
    }

    [Fact]
    public void Asset_without_device_is_missing_serial_when_required() =>
        Assert.True(new Asset { AssetType = new AssetType { RequiresSerialNumber = true } }.IsMissingRequiredSerialNumber);

    [Fact]
    public void Current_valuation_is_the_most_recent_one()
    {
        var asset = new Asset();
        asset.Valuations.AddRange([
            new AssetValuation { ValuedOn = new DateOnly(2024, 1, 1), Amount = 100 },
            new AssetValuation { ValuedOn = new DateOnly(2025, 6, 1), Amount = 80 },
            new AssetValuation { ValuedOn = new DateOnly(2023, 1, 1), Amount = 120 }]);
        Assert.Equal(80, asset.CurrentValuation!.Amount);
    }
}
