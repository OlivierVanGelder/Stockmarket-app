namespace Backend_Example.Logic.Classes
{
    public class CandleItem
    {
        public double Open { get; }
        public double Close { get; }
        public double High { get; }
        public double Low { get; }
        public double Volume { get; }

        public CandleItem(double open, double close, double high, double low, double volume)
        {
            Open = open;
            Close = close;
            High = high;
            Low = low;
            Volume = volume;
        }
    }
}
