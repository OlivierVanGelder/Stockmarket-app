using System.Diagnostics;
using Logic.Models;
using Logic.Stocks;
using Logic.Interfaces;

namespace Logic.Functions
{
    public class StockWritingInterval
    {
        public static async Task WriteStocks(IStockDal stockDal)
        {
            await Task.Run(() =>
            {
                var names = stockDal.GetStockNames();
                var currentDateDigit = Converter.ConvertDateToDigit(
                    DateTime
                        .Now.AddSeconds(-DateTime.Now.Second)
                        .AddMilliseconds(-DateTime.Now.Millisecond)
                );
                foreach (var name in names)
                {
                    var lastStockDigit = Converter.ConvertDateToDigit(stockDal.GetLastStockDate(name));
                    double mS = Converter.ConvertWordToNumber(name) + 1;
                    CandleItem[] newStock = CandleStock.CreateCandleValues(
                        mS,
                        lastStockDigit,
                        currentDateDigit,
                        0.00069444444
                    );
                    stockDal.WriteStocks(newStock, name);
                    stockDal.DeleteDuplicateStocks();
                }
            });
        }
    }
}
