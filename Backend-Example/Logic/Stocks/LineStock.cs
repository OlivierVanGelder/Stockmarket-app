using Logic.Interfaces;
using Logic.Models;

namespace Logic.Stocks;

public class LineStock
{
    public static Task<LineItem[]> GetValues(
        string stockName,
        DateTime startTime,
        DateTime endTime,
        TimeSpan interval,
        IStockDAl stockDAl
    )
    {
        var values = stockDAl.GetLineValues(
            stockName,
            startTime,
            endTime,
            interval
        );
        return values;
    }
}  
