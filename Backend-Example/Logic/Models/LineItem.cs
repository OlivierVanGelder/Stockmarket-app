namespace Backend_Example.Logic.Classes
{
    public class LineItem
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public double Value { get; set; } = 0;

        public LineItem(DateTime date, double value)
        {
            Date = date;
            Value = value;
        }
    }
}
