﻿using Logic.Interfaces;

namespace Logic.Functions
{
    public class StockDeletingInterval
    {
        private readonly System.Timers.Timer _timer;

        public StockDeletingInterval(double intervalInSeconds, IStockDAl stockDAl)
        {
            _timer = new System.Timers.Timer(intervalInSeconds * 1000);
            _timer.Elapsed += (sender, e) => DeleteDuplicates(stockDAl);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private static void DeleteDuplicates(IStockDAl stockDAl)
        {
            stockDAl.DeleteDuplicateStocks();
        }
    }
}
