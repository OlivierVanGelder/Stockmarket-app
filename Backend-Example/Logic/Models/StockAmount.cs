namespace Logic.Models;

public class StockAmount
{
    public string Name { get; set; }
    public double Value { get; set; }
    public double Price { get; set; }
    public double TotalValue { get; set; }

    public StockAmount(string name, double value, double price)
    {
        Price = price;
        Name = name;
        Value = value;
        TotalValue = value * price;
    }
}