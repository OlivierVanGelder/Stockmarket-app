﻿using Backend_Example.Logic.Classes;
using DAL.Tables;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.BDaccess
{
    public class StockDAL : IStockDAL
    {
        private readonly Backend_Example.Data.BDaccess.DbStockEngine _context;

        // Constructor injection of DbContext
        public StockDAL(Backend_Example.Data.BDaccess.DbStockEngine context)
        {
            _context = context;
        }

        public DateTime GetLastStockDate()
        {
            var candle = _context.Candles;
            var dates = candle.Select(c => c.Date).AsEnumerable();

            return dates.DefaultIfEmpty(new DateTime(2020, 11, 1, 12, 0, 0)).Max();
        }

        public string[] GetStockNames()
        {
            var stock = _context.Stocks;
            var stockNames = stock.Select(s => s.Ticker).ToArray();
            return stockNames;
        }

        public void WriteStocks(CandleItem[] c, string ticker)
        {
            int stockId = _context
                .Stocks.Where(s => s.Ticker == ticker)
                .Select(s => s.Id)
                .FirstOrDefault();

            var candles = _context.Candles;
            foreach (var item in c)
            {
                var candlestockminute = new CandlestockMinute
                {
                    Stock_Id = stockId,
                    Date = item.Date,
                    Close = Convert.ToInt32(item.Close * 100),
                    High = Convert.ToInt32(item.High * 100),
                    Low = Convert.ToInt32(item.Low * 100),
                    Open = Convert.ToInt32(item.Open * 100),
                    Volume = Convert.ToInt32(item.Volume * 100),
                };
                candles.Add(candlestockminute);
            }
            _context.SaveChanges();
        }

        public async Task<CandleItem[]> GetCandleValues(
            string stockName,
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval
        )
        {
            int stockId = await _context
                .Stocks.Where(s => s.Ticker == stockName)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var candles = await _context
                .Candles.Where(c =>
                    c.Stock_Id == stockId && c.Date >= startDate && c.Date <= endDate
                )
                .OrderBy(c => c.Date)
                .ToListAsync();

            var intervalTicks = interval.Ticks;

            var groupedCandles = candles
                .GroupBy(c => (c.Date.Ticks - startDate.Ticks) / intervalTicks)
                .Select(candleGroup =>
                {
                    var open = 0.0;
                    var close = 0.0;
                    var high = double.MinValue;
                    var low = double.MaxValue;
                    var volume = 0.0;
                    DateTime firstDate = DateTime.MinValue;
                    bool isFirst = true;

                    foreach (var c in candleGroup)
                    {
                        if (isFirst)
                        {
                            open = c.Open / 100.0;
                            firstDate = c.Date;
                            isFirst = false;
                        }

                        close = c.Close / 100.0;
                        high = Math.Max(high, c.High / 100.0);
                        low = Math.Min(low, c.Low / 100.0);
                        volume += c.Volume / 100.0;
                    }

                    return new CandleItem(
                        open: open,
                        close: close,
                        high: high,
                        low: low,
                        volume: Math.Round(volume, 2),
                        date: firstDate
                    );
                })
                .ToArray();

            return groupedCandles;
        }

        public async Task<LineItem[]> GetLineValues(
            string stockName,
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval
        )
        {
            int stockId = _context
                .Stocks.Where(s => s.Ticker == stockName)
                .Select(s => s.Id)
                .FirstOrDefault();

            var candles = await _context
                .Candles.Where(c =>
                    c.Stock_Id == stockId && c.Date >= startDate && c.Date <= endDate
                )
                .OrderBy(c => c.Date)
                .ToListAsync();

            var intervalTicks = interval.Ticks;
            var filteredCandles = candles
                .GroupBy(c => (c.Date.Ticks - startDate.Ticks) / intervalTicks)
                .Select(g => g.First())
                .OrderBy(c => c.Date);

            var lines = filteredCandles
                .Select(c => new LineItem(c.Date, c.Close / 100.00))
                .ToArray();
            if (lines.Length == 0)
            {
                return lines;
            }
            lines[lines.Length - 1].Value = candles.LastOrDefault().Close / 100.00;

            return lines;
        }

        public void DeleteDuplicateStocks()
        {
            var allCandles = _context.Candles.ToList();

            var duplicateGroups = allCandles
                .GroupBy(c => new { c.Stock_Id, c.Date })
                .Where(g => g.Count() > 1);

            foreach (var group in duplicateGroups)
            {
                var duplicatesToRemove = group.Skip(1).ToList();
                _context.Candles.RemoveRange(duplicatesToRemove);
            }

            _context.SaveChanges();
        }
    }
}
