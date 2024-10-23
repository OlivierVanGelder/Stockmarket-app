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
    }
}
