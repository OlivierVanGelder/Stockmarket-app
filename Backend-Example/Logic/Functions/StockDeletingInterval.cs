using Logic.Interfaces;

namespace Logic.Functions
{
    public class StockDeletingInterval
    {
        private readonly System.Timers.Timer _timer;

        public StockDeletingInterval(double intervalInSeconds, IStockDAal stockDAal)
        {
            _timer = new System.Timers.Timer(intervalInSeconds * 1000);
            _timer.Elapsed += (sender, e) => DeleteDuplicates(stockDAal);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private static void DeleteDuplicates(IStockDAal stockDAal)
        {
            stockDAal.DeleteDuplicateStocks();
        }
    }
}
