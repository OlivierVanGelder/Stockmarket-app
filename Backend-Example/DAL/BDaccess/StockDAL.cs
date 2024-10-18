using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Interfaces;

namespace DAL.BDaccess
{
    public class StockDAL : StockDALinterface
    {
        public string[] GetStockNames()
        {
            return ["IBM", "ADML", "DMSI", "ASML", "APPL", "MSFT"];
        }
    }
}
