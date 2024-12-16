namespace DAL.DataModels;
public class CandleStick
{
    public double Open { get; }
    public double Close { get; }
    public double High { get; }
    public double Low { get; }
    public double Volume { get; }

    public CandleStick(double open, double close, double high, double low, double volume)
    {
        Open = open;
        Close = close;
        High = high;
        Low = low;
        Volume = volume;
    }
}