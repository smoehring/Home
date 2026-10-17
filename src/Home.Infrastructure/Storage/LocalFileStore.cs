using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Smoehring.Home.Application.Abstractions;

namespace Smoehring.Home.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string Section = "Storage";
    public string LocalPath { get; set; } = "/data/files";
    public long MaxFileSize { get; set; } = 200L * 1024 * 1024;
}

/// <summary>File store on a local path (PVC in k3s). Keys are validated to prevent path traversal.</summary>
public sealed class LocalFileStore(IOptions<StorageOptions> options) : IFileStore
{
    private readonly string _root = Path.GetFullPath(options.Value.LocalPath);
    private readonly long _max = options.Value.MaxFileSize;

    public async Task<StoredFile> SaveAsync(string storageKey, Stream content, CancellationToken ct = default)
    {
        var path = Resolve(storageKey);
        Directory.CreateDirectory(_root);
        if (File.Exists(path)) throw new IOException("A file with this key already exists.");
        var tmp = path + ".tmp";
        try
        {
            using var sha = SHA256.Create();
            long size = 0;
            await using (var target = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true))
            {
                var buffer = new byte[81920];
                int read;
                while ((read = await content.ReadAsync(buffer, ct)) > 0)
                {
                    size += read;
                    if (size > _max) throw new InvalidOperationException($"The file exceeds the maximum size of {_max} bytes.");
                    sha.TransformBlock(buffer, 0, read, null, 0);
                    await target.WriteAsync(buffer.AsMemory(0, read), ct);
                }
            }
            sha.TransformFinalBlock([], 0, 0);
            File.Move(tmp, path);
            return new StoredFile(storageKey, size, Convert.ToHexString(sha.Hash!).ToLowerInvariant());
        }
        finally { if (File.Exists(tmp)) File.Delete(tmp); }
    }

    public Task<Stream?> OpenReadAsync(string storageKey, CancellationToken ct = default)
    {
        var path = Resolve(storageKey);
        return Task.FromResult<Stream?>(File.Exists(path) ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true) : null);
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        var path = Resolve(storageKey);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    private string Resolve(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || key != Path.GetFileName(key) || key.StartsWith('.'))
            throw new ArgumentException("Invalid storage key.", nameof(key));
        var full = Path.GetFullPath(Path.Combine(_root, key));
        if (!full.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.Ordinal)) throw new ArgumentException("Invalid storage key.", nameof(key));
        return full;
    }
}
