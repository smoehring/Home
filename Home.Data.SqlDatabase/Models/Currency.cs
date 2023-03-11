namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Currency
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Symbol { get; set; }
    public int? LanguageId { get; set; }
    public virtual Language? Language { get; set; }
}