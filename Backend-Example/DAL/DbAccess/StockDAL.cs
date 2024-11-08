﻿using System;
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
        public DateTime GetLastStockDate()
        {
            using (var db = new DbContext())
            {
                var candle = db.Candles;
                var dates = candle.Select(c => c.Date).AsEnumerable();

                // Now apply DefaultIfEmpty to ensure a default value if no data is returned
                return dates.DefaultIfEmpty(new DateTime(2024, 11, 1, 12, 0, 0)).Max();
            }
        }

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
                        High = Convert.ToInt32(item.High * 100),
                        Low = Convert.ToInt32(item.Low * 100),
                        Open = Convert.ToInt32(item.Open * 100),
                        Volume = Convert.ToInt32(item.Volume * 100),
                    };
                    db.Candles.Add(candlestockminute);
                }
                db.SaveChanges();
            }
        }

        public CandleItem[] GetCandleValues(
            string stockname,
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval
        )
        {
            using (var db = new DbContext())
            {
                // Get the stock ID for the specified stockname
                int stockId = db
                    .Stocks.Where(s => s.Ticker == stockname)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                if (stockId == 0)
                {
                    throw new InvalidOperationException("Stock not found");
                }

                // Query for CandleStockMinute entries within the date range for the given stock
                var candles = db
                    .Candles.Where(c =>
                        c.Stock_Id == stockId && c.Date >= startDate && c.Date <= endDate
                    )
                    .OrderBy(c => c.Date)
                    .ToList();

                // Filter data based on the interval
                List<CandleItem> filteredCandles = new List<CandleItem>();
                DateTime currentIntervalStart = startDate;

                while (currentIntervalStart <= endDate)
                {
                    var candlesInInterval = candles
                        .Where(c =>
                            c.Date >= currentIntervalStart
                            && c.Date < currentIntervalStart + interval
                        )
                        .OrderBy(c => c.Date)
                        .ToList(); // Get all candles in the interval

                    if (candlesInInterval.Any())
                    {
                        // Combine data for the interval
                        var firstCandle = candlesInInterval.First();
                        var lastCandle = candlesInInterval.Last();
                        double combinedHigh = candlesInInterval.Max(c => c.High) / 100.0;
                        double combinedLow = candlesInInterval.Min(c => c.Low) / 100.0;
                        double combinedVolume = candlesInInterval.Sum(c => c.Volume) / 100.0;

                        filteredCandles.Add(
                            new CandleItem(
                                open: firstCandle.Open / 100.0,
                                close: lastCandle.Close / 100.0,
                                high: combinedHigh,
                                low: combinedLow,
                                volume: combinedVolume,
                                date: currentIntervalStart
                            )
                        );
                    }

                    currentIntervalStart += interval; // Move to the next interval
                }

                return filteredCandles.ToArray();
            }
        }

        public void DeleteDuplicateStocks()
        {
            using (var db = new DbContext())
            {
                var allCandles = db.Candles.ToList();

                var duplicateGroups = allCandles
                    .GroupBy(c => new { c.Stock_Id, c.Date })
                    .Where(g => g.Count() > 1);

                foreach (var group in duplicateGroups)
                {
                    var duplicatesToRemove = group.Skip(1).ToList();
                    db.Candles.RemoveRange(duplicatesToRemove);
                }

                db.SaveChanges();
            }
        }
    }
}
