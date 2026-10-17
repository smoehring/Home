using Smoehring.Home.Data.SqlDatabase.Const;

namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Artwork
{
    public int Id { get; set; }
    public bool IsAdult { get; set; }
    public ArtworkStages Stage { get; set; }
    public virtual ICollection<ArtworkArtist>? Artists { get; set; }
    public virtual ICollection<ArtworkCharacter>? Characters { get; set; }
    public virtual Asset? Asset { get; set; }

}