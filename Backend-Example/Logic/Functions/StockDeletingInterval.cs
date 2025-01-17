using Logic.Interfaces;

namespace Logic.Functions;

public class StockDeletingInterval
{
    private readonly System.Timers.Timer _timer;

    public StockDeletingInterval(double intervalInSeconds, IStockDal stockDal)
    {
        _timer = new System.Timers.Timer(intervalInSeconds * 1000);
        // Qodana suppression: Sender and EventArgs required by event signature
        _timer.Elapsed += (sender, e) => DeleteDuplicates(stockDal);
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private static void DeleteDuplicates(IStockDal stockDal)
    {
        stockDal.DeleteDuplicateStocks();
    }
}
