using Logic.Models;
using Logic.Interfaces;

namespace Logic.Stocks
{
    public class CandleStock
    {
        public static Task<CandleItem[]> GetCandleValues(
            string stock,
            double startX,
            double endX,
            double intervalDays,
            IStockDAal stockDal
        )
        {
            var startDate = Converter.ConvertDigitToDate(startX);
            var endDate = Converter.ConvertDigitToDate(endX);
            var interval = TimeSpan.FromDays(intervalDays);
            Console.WriteLine($"startDate: {startDate} EndDate: {endDate}");
            return stockDal.GetCandleValues(stock, startDate, endDate, interval);
        }

        public static CandleItem[] CreateCandleValues(
            double mS,
            double startX,
            double endX,
            double interval
        )
        {
            // Calculate the number of values based on the range and interval
            var numberOfValues = (int)((endX - startX) / interval) + 1;
            var values = new CandleItem[numberOfValues];

            Parallel.For(
                0,
                numberOfValues,
                i =>
                {
                    var x = startX + i * interval; // x increments by the specified interval
                    var open = Formula.CalculateFormula(x, mS) ?? 0;
                    var close = Formula.CalculateFormula(x + interval, mS) ?? 0;
                    var high = GetHighValue(x, interval, mS); // Use fewer iterations for speed
                    var low = GetLowValue(x, interval, mS); // Use fewer iterations for speed
                    var date = Converter.ConvertDigitToDate(x);
                    var volume = Math.Round(
                        Math.Abs(
                            100 / open * (high - low) * (258 + (Math.Sin(30000 * x) + 1) * mS)
                        ),
                        2
                    );

                    open = Math.Round(open, 2);
                    close = Math.Round(close, 2);
                    high = Math.Round(high, 2);
                    low = Math.Round(low, 2);
                    values[i] = new CandleItem(open, close, high, low, volume, date);
                }
            );

            return values;
        }

        // Reduce iteration count for faster calculations
        private static double GetHighValue(
            double x,
            double interval,
            double mS,
            int samplePoints = 20
        )
        {
            var highValue = double.MinValue;
            var step = interval / samplePoints;

            for (var i = 0; i <= samplePoints; i++)
            {
                var result = Formula.CalculateFormula(x + step * i, mS) ?? 0;
                if (result > highValue)
                    highValue = result;
            }

            return highValue;
        }

        private static double GetLowValue(
            double x,
            double interval,
            double mS,
            int samplePoints = 20
        )
        {
            var lowValue = double.MaxValue;
            var step = interval / samplePoints;

            for (var i = 0; i <= samplePoints; i++)
            {
                var result = Formula.CalculateFormula(x + step * i, mS) ?? 0;
                if (result < lowValue)
                    lowValue = result;
            }

            return lowValue;
        }

        public static string[] GetStockNames(IStockDAal stockDAal)
        {
            return stockDAal.GetStockNames();
        }
    }
}
