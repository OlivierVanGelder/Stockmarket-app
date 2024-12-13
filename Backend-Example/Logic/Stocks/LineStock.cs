using Logic.Interfaces;
using Logic.Models;

namespace Logic.Stocks
{
    public class LineStock
    {
        public Task<LineItem[]> GetValues(
            string stockName,
            DateTime startTime,
            DateTime endTime,
            TimeSpan interval,
            IStockDAal stockDAal
        )
        {
            Task<LineItem[]> values = stockDAal.GetLineValues(
                stockName,
                startTime,
                endTime,
                interval
            );
            return values;
        }
    }
}
