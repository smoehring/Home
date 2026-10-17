using Smoehring.Home.Domain.Assets;

namespace Smoehring.Home.Application.Assets;

public sealed record AssetListItem(Guid Uuid, string Name, string? Brand, string? Type, string State, string? SerialNumber, DateTimeOffset CreatedAt);

public sealed record AssetQuery(string? Search = null, int Page = 1, int PageSize = 25);

public sealed record AssetDetails(
    Guid Uuid, string Name, string? Brand, string? Type, int StateId, string State, string? Location, string? Notes,
    string? SerialNumber, string? ModelNumber, decimal? PurchaseAmount, string? PurchaseCurrency, DateOnly? PurchaseDate,
    string? Vendor, bool MissingRequiredSerialNumber, IReadOnlyList<AssetFileInfo> Files);

public sealed record AssetFileInfo(Guid Uuid, string FileName, string? ContentType, long Size, FileKind Kind);

public sealed record SaveAssetCommand(
    Guid? Uuid, string Name, string? Brand, string? Type, string? Location, string? Notes,
    string? SerialNumber, string? ModelNumber,
    decimal? PurchaseAmount, int? PurchaseCurrencyId, DateOnly? PurchaseDate, string? Vendor);

public sealed record InsuranceReportRow(
    string Name, string? Brand, string? SerialNumber, string? ModelNumber, DateOnly? PurchaseDate,
    decimal? PurchaseAmount, string? Currency, decimal? CurrentValue, string State);

public sealed record ValuationItem(int Id, decimal Amount, string Currency, DateOnly ValuedOn, ValuationReason Reason);

public sealed record AddValuationCommand(Guid AssetUuid, decimal Amount, int CurrencyId, DateOnly ValuedOn, ValuationReason Reason);

public sealed record AssetLabel(Guid Uuid, string Name, string? Brand, string? SerialNumber, DateTimeOffset? PrintedAt);

public sealed record LabelQuery(string? Search = null, bool OnlyUnprinted = false, int Max = 200);
