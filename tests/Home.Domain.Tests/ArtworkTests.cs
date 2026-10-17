using Smoehring.Home.Domain.Artworks;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Domain.Tests;

public class ArtworkTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void First_version_becomes_primary_automatically()
    {
        var artwork = new Artwork();
        Assert.True(artwork.AddVersion("original", false).IsPrimary);
    }

    [Fact]
    public void Only_one_version_can_be_primary()
    {
        var artwork = new Artwork();
        var first = artwork.AddVersion("original", true);
        var second = artwork.AddVersion("alt", true);
        Assert.False(first.IsPrimary);
        Assert.True(second.IsPrimary);
        Assert.Same(second, artwork.PrimaryVersion);
    }

    [Fact]
    public void Non_primary_version_keeps_existing_primary()
    {
        var artwork = new Artwork();
        var first = artwork.AddVersion("original", true);
        artwork.AddVersion("alt", false);
        Assert.Same(first, artwork.PrimaryVersion);
    }

    [Fact]
    public void Version_requires_a_label() => Assert.Throws<DomainException>(() => new Artwork().AddVersion(" ", false));

    [Fact]
    public void Version_remembers_what_it_is_derived_from()
    {
        var artwork = new Artwork();
        var original = artwork.AddVersion("original", true);
        Assert.Same(original, artwork.AddVersion("wip", false, derivedFrom: original).ParentVersion);
    }

    [Fact]
    public void Stage_change_is_recorded_in_history()
    {
        var artwork = new Artwork();
        artwork.ChangeStage(ArtworkStage.Sketch, Now, "first sketch");
        Assert.Equal(ArtworkStage.Sketch, artwork.Stage);
        Assert.Equal(ArtworkStage.Sketch, Assert.Single(artwork.StageHistory).Stage);
    }

    [Fact]
    public void Completing_requires_a_primary_version_with_a_file()
    {
        var artwork = new Artwork();
        Assert.Throws<DomainException>(() => artwork.ChangeStage(ArtworkStage.Complete, Now));
        artwork.AddVersion("original", true);
        Assert.Throws<DomainException>(() => artwork.ChangeStage(ArtworkStage.Complete, Now));
        artwork.PrimaryVersion!.AssetFileId = 42;
        artwork.ChangeStage(ArtworkStage.Complete, Now);
        Assert.Equal(ArtworkStage.Complete, artwork.Stage);
    }

    [Fact]
    public void Total_paid_subtracts_refunds_per_currency()
    {
        var artwork = new Artwork();
        artwork.Payments.AddRange([
            new ArtworkPayment { CurrencyId = 1, Amount = 50, Kind = PaymentKind.Deposit },
            new ArtworkPayment { CurrencyId = 1, Amount = 50, Kind = PaymentKind.Final },
            new ArtworkPayment { CurrencyId = 1, Amount = 20, Kind = PaymentKind.Refund },
            new ArtworkPayment { CurrencyId = 2, Amount = 5, Kind = PaymentKind.Tip }]);
        var totals = artwork.TotalPaidByCurrency();
        Assert.Equal(80, totals[1]);
        Assert.Equal(5, totals[2]);
    }

    [Fact]
    public void Artist_display_name_prefers_the_primary_name()
    {
        var artist = new Artist { Names = [new ArtistName { Name = "alias" }, new ArtistName { Name = "main", IsPrimary = true }] };
        Assert.Equal("main", artist.DisplayName);
    }

    [Theory]
    [InlineData(10, 5, 50.0)]
    [InlineData(10, 20, 100.0)]
    public void Set_completion_is_capped_at_100_percent(int planned, int items, double expected)
    {
        var set = new ArtworkSet { PlannedCount = planned };
        for (var i = 0; i < items; i++) set.Items.Add(new ArtworkSetItem { ArtworkId = i, Position = i });
        Assert.Equal((decimal)expected, set.CompletionPercent);
    }

    [Fact]
    public void Set_without_planned_count_has_no_completion() => Assert.Null(new ArtworkSet().CompletionPercent);
}
