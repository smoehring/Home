namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Media2MediaGroup
{
    public int MediaId { get; set; }
    public virtual Media Media { get; set; }
    public int GroupId { get; set; }
    public virtual MediaGroup Group { get; set; }
    public int Order { get; set; }
}