using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend_Example.Logic.Classes;

namespace Logic.Interfaces
{
    public interface StockDALinterface
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
        DateTime GetLastStockDate();
    }
}
