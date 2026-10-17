namespace Smoehring.Home.Data.SqlDatabase.Models;

public partial class Asset
{
    public int Id { get; set; }
    public Guid Uuid { get; set; }
    public string Name { get; set; }
    public Brand? Brand { get; set; }
    public AssetType? AssetType { get; set; }
    public DateTimeOffset Creation { get; set; }
    public int? DeviceId { get; set; }
    public virtual Device? Device { get; set; }
    public int? PurchaseId { get; set; }
    public virtual Purchase? Purchase { get; set; }
    public int? MediaId { get; set; }
    public virtual Media? Media { get; set; }
    public DateTimeOffset? PrintDate { get; set; }
    public int AssetStateId { get; set; }
    public virtual AssetState AssetState { get; set; }
    public int? ArtworkId { get; set; }
    public virtual Artwork? Artwork { get; set; }
    public ICollection<AssetFile>? Files  { get; set; }
}