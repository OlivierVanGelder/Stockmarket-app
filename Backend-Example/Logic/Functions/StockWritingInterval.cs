using Logic.Models;
using Logic.Stocks;
using Logic.Interfaces;

namespace Logic.Functions
{
    public class StockWritingInterval
    {
        private readonly System.Timers.Timer _timer;

        public StockWritingInterval(double intervalInSeconds, IStockDAl stockDAl)
        {
            _timer = new System.Timers.Timer(intervalInSeconds * 1000);
            _timer.Elapsed += (sender, e) => WriteStocks(stockDAl);
            _timer.AutoReset = false;
            _timer.Enabled = true;
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private static void WriteStocks(IStockDAl stockDAl)
        {
            var names = stockDAl.GetStockNames();
            foreach (var name in names)
            {
                var lastStockDigit = Converter.ConvertDateToDigit(stockDAl.GetLastStockDate());
                var currentDateDigit = Converter.ConvertDateToDigit(
                    DateTime
                        .Now.AddSeconds(-DateTime.Now.Second)
                        .AddMilliseconds(-DateTime.Now.Millisecond)
                );
                Console.WriteLine(
                    "Last stock date: " + Converter.ConvertDigitToDate(lastStockDigit)
                );
                Console.WriteLine(
                    "Current date: " + Converter.ConvertDigitToDate(currentDateDigit)
                );
                double mS = Converter.ConvertWordToNumber(name) + 1;
                CandleItem[] newStock = CandleStock.CreateCandleValues(
                    mS,
                    lastStockDigit,
                    currentDateDigit,
                    0.00069444444
                );
                stockDAl.WriteStocks(newStock, name);
                stockDAl.DeleteDuplicateStocks();
            }
        }
    }
}
