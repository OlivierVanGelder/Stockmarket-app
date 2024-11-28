namespace Backend_Example.Models
{
    public class StockTradeRequest
    {
        public string? UserID { get; set; }
        public int? Amount { get; set; }
        public string? Ticker { get; set; }
        public double? Price { get; set; }
    }
}
