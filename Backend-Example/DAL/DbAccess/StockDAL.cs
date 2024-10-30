using System;
using System.Collections.Generic;
using System.Linq;
using Backend_Example.Data.BDaccess;
using Backend_Example.Data.Models;
using Backend_Example.Logic.Classes;
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

        public void WriteStocks(CandleItem[] c, string ticker)
        {
            using (var db = new DbContext())
            {
                int stockId = db
                    .Stocks.Where(s => s.Ticker == ticker)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                var candles = db.Candles;
                foreach (var item in c)
                {
                    var candlestockminute = new CandleStockMinute
                    {
                        Stock_Id = stockId,
                        Date = item.Date,
                        Close = Convert.ToInt32(item.Close * 100),
                        High = Convert.ToInt32(item.Close * 100),
                        Low = Convert.ToInt32(item.Close * 100),
                        Open = Convert.ToInt32(item.Close * 100),
                        Volume = Convert.ToInt32(item.Close * 100),
                    };
                    db.Candles.Add(candlestockminute);
                    db.SaveChanges();
                }
            }
        }
    }
}
