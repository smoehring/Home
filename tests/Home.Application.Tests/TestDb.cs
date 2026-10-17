using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Application.Abstractions;
using Smoehring.Home.Infrastructure.Persistence;

namespace Smoehring.Home.Application.Tests;

/// <summary>Application services run against the real EF model, backed by the in-memory provider.</summary>
public sealed class TestDb : IApplicationDbContextFactory
{
    private readonly DbContextOptions<HomeDbContext> _options =
        new DbContextOptionsBuilder<HomeDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

    public FakeTime Clock { get; } = new();

    public TestDb()
    {
        using var db = new HomeDbContext(_options, Clock);
        db.Database.EnsureCreated();
    }

    public IApplicationDbContext Create() => new HomeDbContext(_options, Clock);

    public HomeDbContext Raw() => new(_options, Clock);
}

public sealed class FakeTime : TimeProvider
{
    public DateTimeOffset Now { get; set; } = new(2026, 3, 1, 12, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => Now;
}

public sealed class InMemoryFileStore : IFileStore
{
    public Dictionary<string, byte[]> Files { get; } = [];

    public async Task<StoredFile> SaveAsync(string storageKey, Stream content, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, ct);
        Files[storageKey] = ms.ToArray();
        return new StoredFile(storageKey, ms.Length, "hash");
    }

    public Task<Stream?> OpenReadAsync(string storageKey, CancellationToken ct = default) =>
        Task.FromResult<Stream?>(Files.TryGetValue(storageKey, out var b) ? new MemoryStream(b) : null);

    public Task DeleteAsync(string storageKey, CancellationToken ct = default) { Files.Remove(storageKey); return Task.CompletedTask; }
}
