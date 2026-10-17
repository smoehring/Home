namespace Smoehring.Home.Application.Abstractions;

public sealed record StoredFile(string StorageKey, long Size, string Sha256);

/// <summary>Storage port for file content. Files are private and served through an authorised endpoint.</summary>
public interface IFileStore
{
    Task<StoredFile> SaveAsync(string storageKey, Stream content, CancellationToken ct = default);
    Task<Stream?> OpenReadAsync(string storageKey, CancellationToken ct = default);
    Task DeleteAsync(string storageKey, CancellationToken ct = default);
}
