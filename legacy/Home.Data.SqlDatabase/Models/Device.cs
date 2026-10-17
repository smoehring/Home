namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Device
{
    public int Id { get; set; }
    public string? SerialNumber { get; set; }
    public string? ModelNumber { get; set; }
    public string? Description { get; set; }
    public Asset Asset { get; set; }
}