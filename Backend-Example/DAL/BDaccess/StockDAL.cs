using System;
using System.Collections.Generic;
using System.Linq;
using Backend_Example.Data.BDaccess;
using Logic.Interfaces;

namespace DAL.BDaccess
{
    public class StockDAL : StockDALinterface
    {
        public string[] GetStockNames()
        {
            using (var db = new DbContext())
            {
                var stock = db.Stocks;
                return stock.Select(s => s.Ticker).ToArray();
            }
        }
    }
}
