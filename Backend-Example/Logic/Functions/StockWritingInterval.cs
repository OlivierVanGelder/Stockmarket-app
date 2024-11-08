using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;
using Logic.Interfaces;

namespace Logic.Functions
{
    public class StockWritingInterval
    {
        private System.Timers.Timer _timer;

        public StockWritingInterval(double intervalInSeconds, StockDALinterface StockDAL)
        {
            _timer = new System.Timers.Timer(intervalInSeconds * 1000);
            _timer.Elapsed += (sender, e) => WriteStocks(StockDAL);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private static void WriteStocks(StockDALinterface StockDAL)
        {
            string[] names = StockDAL.GetStockNames();
            foreach (string name in names)
            {
                double lastStockDigit = Converter.ConvertDateToDigit(StockDAL.GetLastStockDate());
                double currentDateDigit = Converter.ConvertDateToDigit(
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
                StockDAL.WriteStocks(newStock, name);
            }
        }
    }
}
