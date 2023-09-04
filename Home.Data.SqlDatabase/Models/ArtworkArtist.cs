namespace Smoehring.Home.Data.SqlDatabase.Models;

public class ArtworkArtist
{
    public int Id { get; set; }
    public virtual ICollection<ArtistName>? Names { get; set; }
    public virtual ICollection<ArtistProfile> Profiles { get; set; }
    public virtual ICollection<Artwork>? Artworks { get; set; }
}