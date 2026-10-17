using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Application.Abstractions;
using Smoehring.Home.Application.Common;
using Smoehring.Home.Domain.Artworks;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Application.Artworks;

public sealed record ArtworkListItem(Guid Uuid, string Title, ArtworkStage Stage, bool IsAdult, int VersionCount, IReadOnlyList<string> Artists, IReadOnlyList<string> Characters);
public sealed record ArtworkVersionItem(int Id, string Label, bool IsPrimary, Guid? FileUuid, string? FileName);
public sealed record ArtworkFilter(string? Search = null, ArtworkStage? Stage = null, bool IncludeAdult = false, int Page = 1, int PageSize = 25);
public sealed record CreateArtworkCommand(string Title, bool IsAdult, IReadOnlyList<string> Characters, IReadOnlyList<(int ArtistId, ArtistRole Role)> Artists);
public sealed record AddPaymentCommand(Guid ArtworkUuid, decimal Amount, int CurrencyId, DateOnly PaidOn, PaymentKind Kind, PaymentMethod Method, int? ArtistId, string? Reference);

public class ArtworkService(IApplicationDbContextFactory dbFactory, IFileStore fileStore, TimeProvider clock)
{
    public async Task<PagedResult<ArtworkListItem>> ListAsync(ArtworkFilter filter, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var q = db.Artworks.AsNoTracking().Where(a => a.DeletedAt == null);
        if (!filter.IncludeAdult) q = q.Where(a => !a.IsAdult);
        if (filter.Stage is { } stage) q = q.Where(a => a.Stage == stage);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            q = q.Where(a => a.Title.ToLower().Contains(term) || a.Characters.Any(c => c.Character!.Name.ToLower().Contains(term)));
        }
        var page = Math.Max(1, filter.Page);
        var size = Math.Clamp(filter.PageSize, 1, 200);
        var total = await q.CountAsync(ct);
        var rows = await q.OrderBy(a => a.Title).Skip((page - 1) * size).Take(size)
            .Select(a => new
            {
                a.Uuid, a.Title, a.Stage, a.IsAdult, Versions = a.Versions.Count,
                Artists = a.Credits.SelectMany(c => c.Artist!.Names.Where(n => n.IsPrimary).Select(n => n.Name)).ToList(),
                Characters = a.Characters.Select(c => c.Character!.Name).ToList()
            }).ToListAsync(ct);
        return new PagedResult<ArtworkListItem>(
            rows.Select(r => new ArtworkListItem(r.Uuid, r.Title, r.Stage, r.IsAdult, r.Versions, r.Artists, r.Characters)).ToList(), total, page, size);
    }

    /// <summary>Creates an artwork together with its inventory asset (every artwork is an asset).</summary>
    public async Task<Guid> CreateAsync(CreateArtworkCommand cmd, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cmd.Title)) throw new DomainException("An artwork needs a title.");
        await using var db = dbFactory.Create();
        var asset = new Asset { Name = cmd.Title.Trim() };
        var artwork = new Artwork { Title = cmd.Title.Trim(), IsAdult = cmd.IsAdult, Asset = asset };
        artwork.StageHistory.Add(new ArtworkStageChange { Stage = artwork.Stage, At = clock.GetUtcNow() });
        foreach (var (artistId, role) in cmd.Artists) artwork.Credits.Add(new ArtworkArtist { ArtistId = artistId, Role = role });
        foreach (var name in cmd.Characters.Select(c => c.Trim()).Where(c => c.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var lower = name.ToLower();
            var character = await db.Characters.FirstOrDefaultAsync(c => c.Name.ToLower() == lower, ct)
                ?? db.Characters.Add(new Character { Name = name }).Entity;
            artwork.Characters.Add(new ArtworkCharacter { Character = character });
        }
        db.Artworks.Add(artwork);
        await db.SaveChangesAsync(ct);
        return artwork.Uuid;
    }

    public async Task ChangeStageAsync(Guid uuid, ArtworkStage stage, string? note, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var artwork = await Load(db, uuid, ct);
        artwork.ChangeStage(stage, clock.GetUtcNow(), note);
        await db.SaveChangesAsync(ct);
    }

    public async Task AddVersionAsync(Guid uuid, string label, bool isPrimary, int? assetFileId, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var artwork = await Load(db, uuid, ct);
        artwork.AddVersion(label, isPrimary, assetFileId);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Stores the uploaded image as a file of the artwork's asset and adds a version pointing at it.</summary>
    public async Task AddVersionWithFileAsync(Guid uuid, string label, bool isPrimary, string fileName, string? contentType, Stream content, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new DomainException("A version needs a label.");
        var name = Path.GetFileName(fileName?.Replace('\\', '/') ?? "");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("The file needs a name.");
        await using var db = dbFactory.Create();
        var artwork = await Load(db, uuid, ct);
        var key = $"{uuid:N}x{Guid.NewGuid():N}{Path.GetExtension(name)}";
        var stored = await fileStore.SaveAsync(key, content, ct);
        try
        {
            var file = new AssetFile
            {
                AssetId = artwork.AssetId, OriginalFileName = name, StorageKey = stored.StorageKey, ContentType = contentType,
                Size = stored.Size, Sha256 = stored.Sha256, Kind = FileKind.Digital
            };
            db.AssetFiles.Add(file);
            await db.SaveChangesAsync(ct);
            artwork.AddVersion(label, isPrimary, file.Id);
            await db.SaveChangesAsync(ct);
        }
        catch { await fileStore.DeleteAsync(stored.StorageKey, CancellationToken.None); throw; }
    }

    public async Task<IReadOnlyList<ArtworkVersionItem>> VersionsAsync(Guid uuid, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var artwork = await db.Artworks.AsNoTracking().Include(a => a.Versions)
            .FirstOrDefaultAsync(a => a.Uuid == uuid && a.DeletedAt == null, ct) ?? throw new NotFoundException("Artwork");
        var fileIds = artwork.Versions.Where(v => v.AssetFileId != null).Select(v => v.AssetFileId!.Value).ToList();
        var files = await db.AssetFiles.AsNoTracking().Where(f => fileIds.Contains(f.Id)).ToDictionaryAsync(f => f.Id, ct);
        return artwork.Versions.OrderByDescending(v => v.IsPrimary).ThenBy(v => v.Id)
            .Select(v => files.TryGetValue(v.AssetFileId ?? 0, out var f)
                ? new ArtworkVersionItem(v.Id, v.Label, v.IsPrimary, f.Uuid, f.OriginalFileName)
                : new ArtworkVersionItem(v.Id, v.Label, v.IsPrimary, null, null)).ToList();
    }

    public async Task AddPaymentAsync(AddPaymentCommand cmd, CancellationToken ct = default)
    {
        if (cmd.Amount <= 0) throw new DomainException("A payment must be greater than zero.");
        await using var db = dbFactory.Create();
        var artwork = await Load(db, cmd.ArtworkUuid, ct);
        artwork.Payments.Add(new ArtworkPayment
        {
            Amount = cmd.Amount, CurrencyId = cmd.CurrencyId, PaidOn = cmd.PaidOn, Kind = cmd.Kind,
            Method = cmd.Method, ArtistId = cmd.ArtistId, Reference = cmd.Reference
        });
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyDictionary<string, decimal>> TotalPaidAsync(Guid uuid, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var artwork = await db.Artworks.AsNoTracking().Include(a => a.Payments).ThenInclude(p => p.Currency)
            .FirstOrDefaultAsync(a => a.Uuid == uuid, ct) ?? throw new NotFoundException("Artwork");
        return artwork.Payments.GroupBy(p => p.Currency!.Code)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Kind == PaymentKind.Refund ? -p.Amount : p.Amount));
    }

    public async Task<IReadOnlyList<Artist>> ArtistsAsync(CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        return await db.Artists.AsNoTracking().Include(a => a.Names).Include(a => a.Profiles).OrderBy(a => a.Id).ToListAsync(ct);
    }

    public async Task<int> CreateArtistAsync(string name, string? profileUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("An artist needs a name.");
        await using var db = dbFactory.Create();
        var artist = new Artist { Names = [new ArtistName { Name = name.Trim(), IsPrimary = true }] };
        if (!string.IsNullOrWhiteSpace(profileUrl))
        {
            if (!Uri.TryCreate(profileUrl.Trim(), UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
                throw new DomainException("The profile URL must be an http(s) URL.");
            artist.Profiles.Add(new ArtistProfile { Type = uri.Host, Url = uri.ToString() });
        }
        db.Artists.Add(artist);
        await db.SaveChangesAsync(ct);
        return artist.Id;
    }

    public async Task<IReadOnlyList<Character>> CharactersAsync(CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        return await db.Characters.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ArtworkSet>> SetsAsync(CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        return await db.ArtworkSets.AsNoTracking().Include(s => s.Items).OrderBy(s => s.Name).ToListAsync(ct);
    }

    public async Task<int> CreateSetAsync(string name, int? plannedCount, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("A set needs a name.");
        await using var db = dbFactory.Create();
        var set = new ArtworkSet { Name = name.Trim(), PlannedCount = plannedCount };
        db.ArtworkSets.Add(set);
        await db.SaveChangesAsync(ct);
        return set.Id;
    }

    public async Task AddToSetAsync(int setId, Guid artworkUuid, int position, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var artwork = await db.Artworks.FirstOrDefaultAsync(a => a.Uuid == artworkUuid && a.DeletedAt == null, ct) ?? throw new NotFoundException("Artwork");
        var set = await db.ArtworkSets.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == setId, ct) ?? throw new NotFoundException("Set");
        if (set.Items.Any(i => i.ArtworkId == artwork.Id)) return;
        set.Items.Add(new ArtworkSetItem { ArtworkId = artwork.Id, Position = position });
        await db.SaveChangesAsync(ct);
    }

    private static async Task<Artwork> Load(IApplicationDbContext db, Guid uuid, CancellationToken ct) =>
        await db.Artworks.Include(a => a.Versions).Include(a => a.StageHistory).Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.Uuid == uuid && a.DeletedAt == null, ct) ?? throw new NotFoundException("Artwork");
}
