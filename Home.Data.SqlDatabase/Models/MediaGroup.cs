namespace Smoehring.Home.Data.SqlDatabase.Models;

public class MediaGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Media2MediaGroup> Mediae { get; set; }
}