using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;
using Logic.Interfaces;

namespace Logic.Functions
{
    public class StockDeletingInterval
    {
        private System.Timers.Timer _timer;

        public StockDeletingInterval(double intervalInSeconds, StockDALinterface StockDAL)
        {
            _timer = new System.Timers.Timer(intervalInSeconds * 1000);
            _timer.Elapsed += (sender, e) => DeleteDuplicates(StockDAL);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private static void DeleteDuplicates(StockDALinterface StockDAL)
        {
            StockDAL.DeleteDuplicateStocks();
        }
    }
}
