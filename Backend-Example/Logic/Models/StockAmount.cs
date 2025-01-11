namespace Logic.Models;

public class StockAmounts
{
    public string Name { get; set; }
    public double Value { get; set; } = 0;

    public StockAmounts(string name, double value)
    {
        Name = name;
        Value = value;
    }
}