namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Asset
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
}