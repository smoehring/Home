using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Application.Abstractions;
using Smoehring.Home.Application.Common;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Application.Assets;

public class AssetService(IApplicationDbContextFactory dbFactory, IFileStore fileStore, TimeProvider clock)
{
    public const long MaxFileSize = 200L * 1024 * 1024;

    public async Task<PagedResult<AssetListItem>> SearchAsync(AssetQuery query, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var page = Math.Max(1, query.Page);
        var size = Math.Clamp(query.PageSize, 1, 200);
        var q = db.Assets.AsNoTracking().Where(a => a.DeletedAt == null);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(a => a.Name.ToLower().Contains(term)
                || (a.Device != null && a.Device.SerialNumber != null && a.Device.SerialNumber.ToLower().Contains(term))
                || (a.Brand != null && a.Brand.Name.ToLower().Contains(term)));
        }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(a => a.Name).Skip((page - 1) * size).Take(size)
            .Select(a => new AssetListItem(a.Uuid, a.Name, a.Brand != null ? a.Brand.Name : null,
                a.AssetType != null ? a.AssetType.Name : null, a.AssetState!.Name,
                a.Device != null ? a.Device.SerialNumber : null, a.CreatedAt))
            .ToListAsync(ct);
        return new PagedResult<AssetListItem>(items, total, page, size);
    }

    public async Task<AssetDetails?> GetAsync(Guid uuid, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var a = await db.Assets.AsNoTracking()
            .Include(x => x.Brand).Include(x => x.AssetType).Include(x => x.AssetState)
            .Include(x => x.Device).Include(x => x.Purchase).ThenInclude(p => p!.Currency)
            .Include(x => x.Files)
            .FirstOrDefaultAsync(x => x.Uuid == uuid && x.DeletedAt == null, ct);
        return a is null ? null : new AssetDetails(
            a.Uuid, a.Name, a.Brand?.Name, a.AssetType?.Name, a.AssetStateId, a.AssetState!.Name, a.Location, a.Notes,
            a.Device?.SerialNumber, a.Device?.ModelNumber, a.Purchase?.Amount, a.Purchase?.Currency?.Code,
            a.Purchase?.PurchaseDate, a.Purchase?.Vendor, a.IsMissingRequiredSerialNumber,
            a.Files.Select(f => new AssetFileInfo(f.Uuid, f.OriginalFileName, f.ContentType, f.Size, f.Kind)).ToList());
    }

    public async Task<Guid> SaveAsync(SaveAssetCommand cmd, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cmd.Name)) throw new DomainException("An asset needs a name.");
        await using var db = dbFactory.Create();
        Asset asset;
        if (cmd.Uuid is { } id)
        {
            asset = await db.Assets.Include(a => a.Device).Include(a => a.Purchase)
                .FirstOrDefaultAsync(a => a.Uuid == id && a.DeletedAt == null, ct) ?? throw new NotFoundException("Asset");
        }
        else
        {
            asset = new Asset();
            db.Assets.Add(asset);
        }

        asset.Name = cmd.Name.Trim();
        asset.Location = Clean(cmd.Location);
        asset.Notes = Clean(cmd.Notes);
        asset.Brand = await GetOrCreateBrandAsync(db, cmd.Brand, ct);
        asset.AssetType = await GetOrCreateTypeAsync(db, cmd.Type, ct);

        if (Clean(cmd.SerialNumber) is null && Clean(cmd.ModelNumber) is null) asset.Device = null;
        else
        {
            asset.Device ??= new Device();
            asset.Device.SerialNumber = Clean(cmd.SerialNumber);
            asset.Device.ModelNumber = Clean(cmd.ModelNumber);
        }

        if (cmd.PurchaseAmount is { } amount && cmd.PurchaseCurrencyId is { } currencyId && cmd.PurchaseDate is { } date)
        {
            if (amount < 0) throw new DomainException("The purchase amount must not be negative.");
            asset.Purchase ??= new Purchase();
            asset.Purchase.Amount = amount;
            asset.Purchase.CurrencyId = currencyId;
            asset.Purchase.PurchaseDate = date;
            asset.Purchase.Vendor = Clean(cmd.Vendor);
        }
        else asset.Purchase = null;

        await db.SaveChangesAsync(ct);
        return asset.Uuid;
    }

    public async Task ChangeStateAsync(Guid uuid, int newStateId, string? note, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var asset = await db.Assets.Include(a => a.StateChanges).FirstOrDefaultAsync(a => a.Uuid == uuid && a.DeletedAt == null, ct)
            ?? throw new NotFoundException("Asset");
        if (!await db.AssetStates.AnyAsync(s => s.Id == newStateId, ct)) throw new NotFoundException("Asset state");
        asset.ChangeState(newStateId, clock.GetUtcNow(), note);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid uuid, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var asset = await db.Assets.FirstOrDefaultAsync(a => a.Uuid == uuid && a.DeletedAt == null, ct) ?? throw new NotFoundException("Asset");
        asset.DeletedAt = clock.GetUtcNow();
        await db.SaveChangesAsync(ct);
    }

    public async Task<AssetFileInfo> AddFileAsync(Guid assetUuid, string fileName, string? contentType, FileKind kind, Stream content, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var asset = await db.Assets.FirstOrDefaultAsync(a => a.Uuid == assetUuid && a.DeletedAt == null, ct) ?? throw new NotFoundException("Asset");
        var name = Path.GetFileName(fileName?.Replace('\\', '/') ?? "");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("The file needs a name.");
        var key = $"{assetUuid:N}x{Guid.NewGuid():N}{Path.GetExtension(name)}";
        var stored = await fileStore.SaveAsync(key, content, ct);
        var file = new AssetFile
        {
            AssetId = asset.Id, OriginalFileName = name, StorageKey = stored.StorageKey, ContentType = contentType,
            Size = stored.Size, Sha256 = stored.Sha256, Kind = kind
        };
        db.AssetFiles.Add(file);
        try { await db.SaveChangesAsync(ct); }
        catch { await fileStore.DeleteAsync(stored.StorageKey, CancellationToken.None); throw; }
        return new AssetFileInfo(file.Uuid, file.OriginalFileName, file.ContentType, file.Size, file.Kind);
    }

    public async Task<(AssetFile File, Stream Content)?> OpenFileAsync(Guid fileUuid, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var file = await db.AssetFiles.AsNoTracking().FirstOrDefaultAsync(f => f.Uuid == fileUuid, ct);
        if (file is null) return null;
        var stream = await fileStore.OpenReadAsync(file.StorageKey, ct);
        return stream is null ? null : (file, stream);
    }

    public async Task<IReadOnlyList<ValuationItem>> ValuationsAsync(Guid assetUuid, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var asset = await db.Assets.AsNoTracking().Include(a => a.Valuations).ThenInclude(v => v.Currency)
            .FirstOrDefaultAsync(a => a.Uuid == assetUuid && a.DeletedAt == null, ct) ?? throw new NotFoundException("Asset");
        return asset.Valuations.OrderByDescending(v => v.ValuedOn).ThenByDescending(v => v.Id)
            .Select(v => new ValuationItem(v.Id, v.Amount, v.Currency?.Code ?? "", v.ValuedOn, v.Reason)).ToList();
    }

    public async Task AddValuationAsync(AddValuationCommand cmd, CancellationToken ct = default)
    {
        if (cmd.Amount < 0) throw new DomainException("A valuation must not be negative.");
        await using var db = dbFactory.Create();
        var asset = await db.Assets.Include(a => a.Valuations).FirstOrDefaultAsync(a => a.Uuid == cmd.AssetUuid && a.DeletedAt == null, ct)
            ?? throw new NotFoundException("Asset");
        if (!await db.Currencies.AnyAsync(c => c.Id == cmd.CurrencyId, ct)) throw new NotFoundException("Currency");
        asset.Valuations.Add(new AssetValuation { Amount = cmd.Amount, CurrencyId = cmd.CurrencyId, ValuedOn = cmd.ValuedOn, Reason = cmd.Reason });
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveValuationAsync(Guid assetUuid, int valuationId, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var asset = await db.Assets.Include(a => a.Valuations).FirstOrDefaultAsync(a => a.Uuid == assetUuid && a.DeletedAt == null, ct)
            ?? throw new NotFoundException("Asset");
        var valuation = asset.Valuations.FirstOrDefault(v => v.Id == valuationId) ?? throw new NotFoundException("Valuation");
        asset.Valuations.Remove(valuation);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<AssetLabel>> LabelsAsync(LabelQuery query, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var q = db.Assets.AsNoTracking().Where(a => a.DeletedAt == null);
        if (query.OnlyUnprinted) q = q.Where(a => a.PrintDate == null);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(a => a.Name.ToLower().Contains(term)
                || (a.Device != null && a.Device.SerialNumber != null && a.Device.SerialNumber.ToLower().Contains(term)));
        }
        return await q.OrderBy(a => a.Name).Take(Math.Clamp(query.Max, 1, 500))
            .Select(a => new AssetLabel(a.Uuid, a.Name, a.Brand != null ? a.Brand.Name : null,
                a.Device != null ? a.Device.SerialNumber : null, a.PrintDate))
            .ToListAsync(ct);
    }

    public async Task MarkPrintedAsync(IReadOnlyCollection<Guid> uuids, CancellationToken ct = default)
    {
        if (uuids.Count == 0) return;
        await using var db = dbFactory.Create();
        var now = clock.GetUtcNow();
        var assets = await db.Assets.Where(a => uuids.Contains(a.Uuid) && a.DeletedAt == null).ToListAsync(ct);
        foreach (var a in assets) a.PrintDate = now;
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<InsuranceReportRow>> InsuranceReportAsync(CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var assets = await db.Assets.AsNoTracking()
            .Include(a => a.Brand).Include(a => a.AssetState).Include(a => a.Device)
            .Include(a => a.Purchase).ThenInclude(p => p!.Currency).Include(a => a.Valuations)
            .Where(a => a.DeletedAt == null && a.AssetState!.Ownership)
            .OrderBy(a => a.Name).ToListAsync(ct);
        return assets.Select(a => new InsuranceReportRow(a.Name, a.Brand?.Name, a.Device?.SerialNumber, a.Device?.ModelNumber,
            a.Purchase?.PurchaseDate, a.Purchase?.Amount, a.Purchase?.Currency?.Code,
            a.CurrentValuation?.Amount ?? a.Purchase?.Amount, a.AssetState!.Name)).ToList();
    }

    public static string ToCsv(IEnumerable<InsuranceReportRow> rows)
    {
        var sb = new StringBuilder("Name,Brand,SerialNumber,ModelNumber,PurchaseDate,PurchaseAmount,Currency,CurrentValue,State\n");
        foreach (var r in rows)
            sb.AppendJoin(',', Csv(r.Name), Csv(r.Brand), Csv(r.SerialNumber), Csv(r.ModelNumber),
                Csv(r.PurchaseDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                Csv(r.PurchaseAmount?.ToString(CultureInfo.InvariantCulture)), Csv(r.Currency),
                Csv(r.CurrentValue?.ToString(CultureInfo.InvariantCulture)), Csv(r.State)).Append('\n');
        return sb.ToString();
    }

    /// <summary>Escapes a CSV field, including protection against spreadsheet formula injection.</summary>
    internal static string Csv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value[0] is '=' or '+' or '-' or '@' or '\t' or '\r') value = "'" + value;
        return value.IndexOfAny([',', '"', '\n', '\r']) >= 0 ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
    }

    private static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static async Task<Brand?> GetOrCreateBrandAsync(IApplicationDbContext db, string? name, CancellationToken ct)
    {
        name = Clean(name);
        if (name is null) return null;
        var lower = name.ToLower();
        return await db.Brands.FirstOrDefaultAsync(b => b.Name.ToLower() == lower, ct) ?? db.Brands.Add(new Brand { Name = name }).Entity;
    }

    private static async Task<AssetType?> GetOrCreateTypeAsync(IApplicationDbContext db, string? name, CancellationToken ct)
    {
        name = Clean(name);
        if (name is null) return null;
        var lower = name.ToLower();
        return await db.AssetTypes.FirstOrDefaultAsync(b => b.Name.ToLower() == lower, ct) ?? db.AssetTypes.Add(new AssetType { Name = name }).Entity;
    }
}
