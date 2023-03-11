namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Media
{
    public int Id { get; set; }
    public MediaType MediaType { get; set; }
    public Asset Asset { get; set; }
    public virtual ICollection<Media2MediaGroup> Groups { get; set; }
}