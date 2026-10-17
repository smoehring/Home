using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Domain.Artworks;

public enum ArtworkStage
{
    Unknown = 0, Backlog = 1, InProgress = 2, Complete = 3, Archived = 4,
    Commissioned = 5, Sketch = 6, Lineart = 7, Colored = 8
}

public enum ArtistRole { Other = 0, Sketch = 1, Lineart = 2, Color = 3, Background = 4, Commission = 5 }

public enum PaymentKind { Final = 0, Deposit = 1, Tip = 2, Refund = 3 }

public enum PaymentMethod { Other = 0, PayPal = 1, Bank = 2, Cash = 3 }

public class Artwork : AuditableEntity
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public bool IsAdult { get; set; }
    public ArtworkStage Stage { get; private set; } = ArtworkStage.Backlog;

    public List<ArtworkArtist> Credits { get; set; } = [];
    public List<ArtworkCharacter> Characters { get; set; } = [];
    public List<ArtworkPayment> Payments { get; set; } = [];
    public List<ArtworkVersion> Versions { get; set; } = [];
    public List<ArtworkStageChange> StageHistory { get; set; } = [];
    public List<ArtworkSetItem> SetItems { get; set; } = [];

    public ArtworkVersion? PrimaryVersion => Versions.FirstOrDefault(v => v.IsPrimary);

    /// <summary>Net amounts paid per currency (refunds are subtracted).</summary>
    public IReadOnlyDictionary<int, decimal> TotalPaidByCurrency() =>
        Payments.GroupBy(p => p.CurrencyId)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Kind == PaymentKind.Refund ? -p.Amount : p.Amount));

    public void ChangeStage(ArtworkStage newStage, DateTimeOffset at, string? note = null)
    {
        if (newStage == Stage) return;
        if (newStage == ArtworkStage.Complete && PrimaryVersion?.AssetFileId is null)
            throw new DomainException("An artwork can only be completed when it has a primary version with a file.");
        StageHistory.Add(new ArtworkStageChange { Stage = newStage, At = at, Note = note });
        Stage = newStage;
    }

    /// <summary>Adds a version; only one version can be primary at a time.</summary>
    public ArtworkVersion AddVersion(string label, bool isPrimary, int? assetFileId = null, ArtworkVersion? derivedFrom = null)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new DomainException("A version needs a label.");
        if (isPrimary) foreach (var v in Versions) v.IsPrimary = false;
        var version = new ArtworkVersion
        {
            Label = label, IsPrimary = isPrimary || Versions.Count == 0, AssetFileId = assetFileId, ParentVersion = derivedFrom
        };
        Versions.Add(version);
        return version;
    }
}

public class Artist : Entity
{
    public List<ArtistName> Names { get; set; } = [];
    public List<ArtistProfile> Profiles { get; set; } = [];
    public string DisplayName => (Names.FirstOrDefault(n => n.IsPrimary) ?? Names.FirstOrDefault())?.Name ?? $"Artist #{Id}";
}

public class ArtistName : Entity
{
    public int ArtistId { get; set; }
    public string Name { get; set; } = "";
    public bool IsPrimary { get; set; }
}

public class ArtistProfile : Entity
{
    public int ArtistId { get; set; }
    public string Type { get; set; } = "";
    public string Url { get; set; } = "";
}

public class ArtworkArtist
{
    public int ArtworkId { get; set; }
    public int ArtistId { get; set; }
    public Artist? Artist { get; set; }
    public ArtistRole Role { get; set; }
}

public class Character : Entity
{
    public string Name { get; set; } = "";
    public string? Series { get; set; }
    public bool IsOwned { get; set; }
    public string? Notes { get; set; }
}

public class ArtworkCharacter
{
    public int ArtworkId { get; set; }
    public int CharacterId { get; set; }
    public Character? Character { get; set; }
}

public class ArtworkPayment : Entity
{
    public int ArtworkId { get; set; }
    public int? ArtistId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public Currency? Currency { get; set; }
    public DateOnly PaidOn { get; set; }
    public PaymentKind Kind { get; set; }
    public PaymentMethod Method { get; set; }
    public string? Reference { get; set; }
    public int? InvoiceFileId { get; set; }
}

public class ArtworkVersion : Entity
{
    public int ArtworkId { get; set; }
    public string Label { get; set; } = "";
    public bool IsPrimary { get; set; }
    public int? ParentVersionId { get; set; }
    public ArtworkVersion? ParentVersion { get; set; }
    public int? AssetFileId { get; set; }
    public string? Notes { get; set; }
}

public class ArtworkStageChange : Entity
{
    public int ArtworkId { get; set; }
    public ArtworkStage Stage { get; set; }
    public DateTimeOffset At { get; set; }
    public string? Note { get; set; }
}

public class ArtworkSet : Entity
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int? PlannedCount { get; set; }
    public List<ArtworkSetItem> Items { get; set; } = [];

    /// <summary>Completion in percent (0-100), or null when no planned count is known.</summary>
    public decimal? CompletionPercent =>
        PlannedCount is > 0 ? Math.Min(100m, Math.Round(Items.Count * 100m / PlannedCount.Value, 1)) : null;
}

public class ArtworkSetItem
{
    public int SetId { get; set; }
    public ArtworkSet? Set { get; set; }
    public int ArtworkId { get; set; }
    public Artwork? Artwork { get; set; }
    public int Position { get; set; }
    public int? VariantOfPosition { get; set; }
}
