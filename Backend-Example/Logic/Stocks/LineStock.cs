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
            IStockDAal stockDAal
        )
        {
            var values = stockDAal.GetLineValues(
                stockName,
                startTime,
                endTime,
                interval
            );
            return values;
        }
    }
