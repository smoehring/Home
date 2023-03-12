namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Media
{
    public int Id { get; set; }
    public Asset Asset { get; set; }
    public virtual MediaGroup? Group { get; set; }
    public int? GroupOrder { get; set; }

    public virtual ICollection<MediaName>? MediaNames { get; set; }
}