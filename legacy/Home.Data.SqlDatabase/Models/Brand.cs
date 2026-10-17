namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Brand  
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Asset> Assets { get; set; }
}