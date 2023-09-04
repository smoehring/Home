namespace Smoehring.Home.Data.SqlDatabase.Models;

public class ArtworkCharacters
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOwned { get; set; }
    public virtual ICollection<Artwork>? Artworks { get; set; }
}