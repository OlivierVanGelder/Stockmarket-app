namespace DAL.Tables;
public class Stock
{
    public int Id { get; set; }
    public string Ticker { get; set; } = "";
    public int PriceInCents { get; set; }
}
