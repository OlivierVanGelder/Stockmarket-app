using DAL.Tables;
using Backend_Example.Logic.Classes;
using Logic.Interfaces;

namespace DAL.BDaccess
{
    public class StockDAL : StockDALinterface
    {
        private readonly Backend_Example.Data.BDaccess.DbContext _context;

        // Constructor injection of DbContext
        public StockDAL(Backend_Example.Data.BDaccess.DbContext context)
        {
            _context = context;
        }

        public DateTime GetLastStockDate()
        {
            var candle = _context.Candles;
            var dates = candle.Select(c => c.Date).AsEnumerable();

            // Now apply DefaultIfEmpty to ensure a default value if no data is returned
            return dates.DefaultIfEmpty(new DateTime(2020, 11, 1, 12, 0, 0)).Max();
        }

        public string[] GetStockNames()
        {
            var stock = _context.Stocks;
            return stock.Select(s => s.Ticker).ToArray();
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
        string stockName, DateTime startDate, DateTime endDate, TimeSpan interval
    )
        {
            using (var db = _context)
            {
                int stockId = db
                    .Stocks.Where(s => s.Ticker == stockName)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                var candles = db
                    .Candles.Where(c =>
                        c.Stock_Id == stockId && c.Date >= startDate && c.Date <= endDate
                    )
                    .OrderBy(c => c.Date)
                    .AsEnumerable()
                    .GroupBy(c => c.Date.Ticks / interval.Ticks);

                var filteredCandles = candles.Select(candleGroup => new CandleItem(
                    open: candleGroup.First().Open / 100.0,
                    close: candleGroup.Last().Close / 100.0,
                    high: candleGroup.Max(c => c.High) / 100.0,
                    low: candleGroup.Min(c => c.Low) / 100.0,
                    volume: candleGroup.Sum(c => c.Volume) / 100.0,
                    date: candleGroup.First().Date
                )).ToArray();

                return filteredCandles;
            }
        }

        public async Task<LineItem[]> GetLineValues(string stockName, DateTime startDate, DateTime endDate, TimeSpan interval)
        {
            using (var db = _context)
            {
                int stockId = db
                    .Stocks.Where(s => s.Ticker == stockName)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                var candles = db
                    .Candles.Where(c =>
                        c.Stock_Id == stockId && c.Date >= startDate && c.Date <= endDate
                    )
                    .OrderBy(c => c.Date);

                var values = candles.Select(c => new LineItem(c.Date, c.Open / 100.0)).ToArray();

                return values;
            }
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
