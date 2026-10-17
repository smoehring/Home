using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Application.Abstractions;
using Smoehring.Home.Application.Common;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.Common;
using Smoehring.Home.Domain.MediaLibrary;

namespace Smoehring.Home.Application.MediaLibrary;

public sealed record MediaQuery(string? Search = null, MediaKind? Kind = null, int Page = 1, int PageSize = 25);

public sealed record MediaListItem(Guid AssetUuid, string Title, MediaKind Kind, string? Group, int? GroupOrder, DateOnly? ReleaseDate);

public sealed record MediaNameItem(string Name, int LanguageId);

public sealed record MediaExternalIdItem(string System, string Value);

public sealed record MediaDetails(
    Guid AssetUuid, string Title, MediaKind Kind, string? Group, int? GroupOrder, DateOnly? ReleaseDate,
    IReadOnlyList<MediaNameItem> Names, IReadOnlyList<MediaExternalIdItem> ExternalIds);

public sealed record SaveMediaCommand(
    Guid? AssetUuid, string Title, MediaKind Kind, string? Group, int? GroupOrder, DateOnly? ReleaseDate,
    IReadOnlyList<MediaNameItem> Names, IReadOnlyList<MediaExternalIdItem> ExternalIds);

/// <summary>Media library use cases. Every media item is an inventory asset with a media extension.</summary>
public class MediaService(IApplicationDbContextFactory dbFactory, TimeProvider clock)
{
    public async Task<PagedResult<MediaListItem>> SearchAsync(MediaQuery query, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var page = Math.Max(1, query.Page);
        var size = Math.Clamp(query.PageSize, 1, 200);
        var q = db.Assets.AsNoTracking().Where(a => a.DeletedAt == null && a.Media != null);
        if (query.Kind is { } kind) q = q.Where(a => a.Media!.Kind == kind);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(a => a.Name.ToLower().Contains(term)
                || a.Media!.Names.Any(n => n.Name.ToLower().Contains(term))
                || a.Media!.ExternalIds.Any(e => e.Value.ToLower().Contains(term)));
        }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(a => a.Media!.Group != null ? a.Media.Group.Name : a.Name)
            .ThenBy(a => a.Media!.GroupOrder).ThenBy(a => a.Name)
            .Skip((page - 1) * size).Take(size)
            .Select(a => new MediaListItem(a.Uuid, a.Name, a.Media!.Kind,
                a.Media.Group != null ? a.Media.Group.Name : null, a.Media.GroupOrder, a.Media.ReleaseDate))
            .ToListAsync(ct);
        return new PagedResult<MediaListItem>(items, total, page, size);
    }

    public async Task<MediaDetails?> GetAsync(Guid assetUuid, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var a = await db.Assets.AsNoTracking()
            .Include(x => x.Media!).ThenInclude(m => m.Group)
            .Include(x => x.Media!).ThenInclude(m => m.Names)
            .Include(x => x.Media!).ThenInclude(m => m.ExternalIds)
            .FirstOrDefaultAsync(x => x.Uuid == assetUuid && x.DeletedAt == null && x.Media != null, ct);
        if (a?.Media is not { } m) return null;
        return new MediaDetails(a.Uuid, a.Name, m.Kind, m.Group?.Name, m.GroupOrder, m.ReleaseDate,
            m.Names.OrderBy(n => n.Id).Select(n => new MediaNameItem(n.Name, n.LanguageId)).ToList(),
            m.ExternalIds.OrderBy(e => e.Id).Select(e => new MediaExternalIdItem(e.System, e.Value)).ToList());
    }

    public async Task<Guid> SaveAsync(SaveMediaCommand cmd, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cmd.Title)) throw new DomainException("A media item needs a title.");
        if (cmd.GroupOrder is < 0) throw new DomainException("The group order must not be negative.");

        var names = cmd.Names.Where(n => !string.IsNullOrWhiteSpace(n.Name))
            .Select(n => new MediaNameItem(n.Name.Trim(), n.LanguageId)).Distinct().ToList();
        var ids = cmd.ExternalIds.Where(e => !string.IsNullOrWhiteSpace(e.System) || !string.IsNullOrWhiteSpace(e.Value)).ToList();
        if (ids.Any(e => string.IsNullOrWhiteSpace(e.System) || string.IsNullOrWhiteSpace(e.Value)))
            throw new DomainException("An external id needs both a system and a value.");
        ids = ids.Select(e => new MediaExternalIdItem(e.System.Trim(), e.Value.Trim())).Distinct().ToList();

        await using var db = dbFactory.Create();
        var languageIds = names.Select(n => n.LanguageId).Distinct().ToList();
        if (languageIds.Count > 0 && await db.Languages.CountAsync(l => languageIds.Contains(l.Id), ct) != languageIds.Count)
            throw new NotFoundException("Language");

        Asset asset;
        if (cmd.AssetUuid is { } id)
        {
            asset = await db.Assets.Include(a => a.Media!).ThenInclude(m => m.Names)
                .Include(a => a.Media!).ThenInclude(m => m.ExternalIds)
                .FirstOrDefaultAsync(a => a.Uuid == id && a.DeletedAt == null && a.Media != null, ct)
                ?? throw new NotFoundException("Media item");
        }
        else
        {
            asset = new Asset { Media = new Media() };
            db.Assets.Add(asset);
        }

        var media = asset.Media!;
        asset.Name = cmd.Title.Trim();
        media.Kind = cmd.Kind;
        media.GroupOrder = cmd.GroupOrder;
        media.ReleaseDate = cmd.ReleaseDate;
        media.Group = await GetOrCreateGroupAsync(db, cmd.Group, ct);
        media.Names.Clear();
        media.Names.AddRange(names.Select(n => new MediaName { Name = n.Name, LanguageId = n.LanguageId }));
        media.ExternalIds.Clear();
        media.ExternalIds.AddRange(ids.Select(e => new MediaExternalId { System = e.System, Value = e.Value }));

        await db.SaveChangesAsync(ct);
        return asset.Uuid;
    }

    public async Task DeleteAsync(Guid assetUuid, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var asset = await db.Assets.FirstOrDefaultAsync(a => a.Uuid == assetUuid && a.DeletedAt == null && a.Media != null, ct)
            ?? throw new NotFoundException("Media item");
        asset.DeletedAt = clock.GetUtcNow();
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Language>> LanguagesAsync(CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        return await db.Languages.AsNoTracking().OrderBy(l => l.DisplayName).ToListAsync(ct);
    }

    private static async Task<MediaGroup?> GetOrCreateGroupAsync(IApplicationDbContext db, string? name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var trimmed = name.Trim();
        var group = await db.MediaGroups.FirstOrDefaultAsync(g => g.Name == trimmed, ct);
        if (group is null) { group = new MediaGroup { Name = trimmed }; db.MediaGroups.Add(group); }
        return group;
    }
}
