using Smoehring.Home.Domain.MediaLibrary;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Domain.Assets;

public enum FileKind { Other = 0, Invoice = 1, Photo = 2, Manual = 3, Scan = 4, Digital = 5, Thumbnail = 6 }

public enum ValuationReason { Purchase = 0, Appraisal = 1, ReplacementValue = 2 }

public class Asset : AuditableEntity
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset? PrintDate { get; set; }

    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }
    public int? AssetTypeId { get; set; }
    public AssetType? AssetType { get; set; }
    public int AssetStateId { get; set; } = AssetState.AvailableId;
    public AssetState? AssetState { get; set; }

    public Device? Device { get; set; }
    public Purchase? Purchase { get; set; }
    public Media? Media { get; set; }

    public List<AssetValuation> Valuations { get; set; } = [];
    public List<AssetStateChange> StateChanges { get; set; } = [];
    public List<AssetFile> Files { get; set; } = [];

    /// <summary>
    /// Records a state change (history is required for loss/theft claims).
    /// </summary>
    public void ChangeState(int newStateId, DateTimeOffset at, string? note = null)
    {
        if (newStateId == AssetStateId) return;
        StateChanges.Add(new AssetStateChange { FromStateId = AssetStateId, ToStateId = newStateId, At = at, Note = note });
        AssetStateId = newStateId;
    }

    /// <summary>True when the asset type demands a serial number and none is recorded.</summary>
    public bool IsMissingRequiredSerialNumber =>
        AssetType is { RequiresSerialNumber: true } && string.IsNullOrWhiteSpace(Device?.SerialNumber);

    /// <summary>Most recent valuation by date, if any.</summary>
    public AssetValuation? CurrentValuation => Valuations.OrderByDescending(v => v.ValuedOn).ThenByDescending(v => v.Id).FirstOrDefault();
}

public class Device : Entity
{
    public int AssetId { get; set; }
    public string? SerialNumber { get; set; }
    public string? ModelNumber { get; set; }
    public string? Description { get; set; }
}

public class Purchase : Entity
{
    public int AssetId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public Currency? Currency { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public string? Vendor { get; set; }
    public string? OrderNumber { get; set; }
    public DateOnly? WarrantyUntil { get; set; }
}

public class AssetValuation : Entity
{
    public int AssetId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public Currency? Currency { get; set; }
    public DateOnly ValuedOn { get; set; }
    public ValuationReason Reason { get; set; }
}

public class AssetStateChange : Entity
{
    public int AssetId { get; set; }
    public int FromStateId { get; set; }
    public int ToStateId { get; set; }
    public DateTimeOffset At { get; set; }
    public string? Note { get; set; }
}

/// <summary>Metadata of a stored file; the content lives behind IFileStore.</summary>
public class AssetFile : Entity
{
    public int AssetId { get; set; }
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public string OriginalFileName { get; set; } = "";
    public string StorageKey { get; set; } = "";
    public string? ContentType { get; set; }
    public long Size { get; set; }
    public string? Sha256 { get; set; }
    public FileKind Kind { get; set; }
}
