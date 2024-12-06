namespace Backend_Example.Models
{
    public class StockTradeRequest
    {
        public int? Amount { get; set; }
        public string? Ticker { get; set; }
        public double? Price { get; set; }
        public string? Action { get; set; }
    }
}
