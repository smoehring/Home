namespace Smoehring.Home.Data.SqlDatabase.Models;

/// <summary>
/// Metadata to identify a File in an Storage Account
/// </summary>
public class AssetFile
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; }
    public string StorageFileName { get; set; }
    public string? ContentType { get; set; }
    public long? Size { get; set; }
    public int? AssetId { get; set; }
    public virtual Asset? Asset { get; set; }
}