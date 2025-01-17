namespace Logic.Models;

public class LineItem
{
    public DateTime Date { get; set; }
    public double Value { get; set; }

    public LineItem(DateTime date, double value)
    {
        Date = date;
        Value = value;
    }
}