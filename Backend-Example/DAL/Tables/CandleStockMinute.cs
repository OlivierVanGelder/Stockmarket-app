namespace DAL.Tables;
public class CandlestockMinute
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public DateTime Date { get; set; }
    public int Open { get; set; }
    public int High { get; set; }
    public int Low { get; set; }
    public int Close { get; set; }
    public int Volume { get; set; }
}
