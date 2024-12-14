namespace Logic.Models;

public class CandleItem
{
    public DateTime Date { get; }
    public double Open { get; }
    public double Close { get; }
    public double High { get; }
    public double Low { get; }
    public double Volume { get; }

    public CandleItem(
        double open,
        double close,
        double high,
        double low,
        double volume,
        DateTime date
    )
    {
        Date = date;
        Open = open;
        Close = close;
        High = high;
        Low = low;
        Volume = volume;
    }
}
