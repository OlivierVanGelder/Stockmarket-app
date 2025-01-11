using Logic.Models;

namespace Logic.Interfaces;
public interface IStockDal
{
    string[] GetStockNames();
    void WriteStocks(CandleItem[] c, string ticker);
    void DeleteDuplicateStocks();

    Task<CandleItem[]> GetCandleValues(
        string stock,
        DateTime startDate,
        DateTime endDate,
        TimeSpan interval
    );
    Task<LineItem[]> GetLineValues(
        string stockname,
        DateTime startDate,
        DateTime endDate,
        TimeSpan interval
    );
    DateTime GetLastStockDate(string stockname);
}