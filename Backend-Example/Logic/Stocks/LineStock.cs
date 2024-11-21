using Backend_Example.Logic.Classes;
using Logic.Interfaces;

namespace Backend_Example.Logic.Stocks
{
    public class LineStock
    {
        public Task<LineItem[]> GetValues(
            string stockName,
            DateTime startTime,
            DateTime endTime,
            TimeSpan interval,
            StockDALinterface stockDal
        )
        {
            Task<LineItem[]> values = stockDal.GetLineValues(stockName, startTime, endTime, interval);
            return values;
        }
    }
}
