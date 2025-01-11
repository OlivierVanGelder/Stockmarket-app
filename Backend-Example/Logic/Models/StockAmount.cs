namespace Logic.Models;

public class StockAmount
{
    public string Name { get; set; }
    public double Value { get; set; } = 0;

    public StockAmount(string name, double value)
    {
        Name = name;
        Value = value;
    }
}