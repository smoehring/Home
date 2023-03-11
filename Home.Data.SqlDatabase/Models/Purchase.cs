namespace Smoehring.Home.Data.SqlDatabase.Models;

public class Purchase
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public DateTimeOffset PurchaseTime { get; set; }
    public Asset Asset { get; set; }
}