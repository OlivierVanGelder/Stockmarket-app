using Backend_Example.Logic.Classes;
using Logic.Interfaces;

namespace Backend_Example.Logic.Stocks
{
    public class CandleStock
    {
        public static CandleItem[] GetCandleValues(
            string stock,
            double startX,
            double endX,
            double intervalDays,
            StockDALinterface stockDAL
        )
        {
            DateTime startDate = Converter.ConvertDigitToDate(startX);
            DateTime endDate = Converter.ConvertDigitToDate(endX);
            TimeSpan interval = TimeSpan.FromDays(intervalDays);
            Console.WriteLine($"startDate: {startDate} EndDate: {endDate}");
            return stockDAL.GetCandleValues(stock, startDate, endDate, interval);
        }

        public static CandleItem[] CreateCandleValues(
            double mS,
            double startX,
            double endX,
            double interval
        )
        {
            // Calculate the number of values based on the range and interval
            int numberOfValues = (int)((endX - startX) / interval) + 1;
            CandleItem[] values = new CandleItem[numberOfValues];

            // Use Parallel.For for potential speed-up by parallelizing loop execution
            Parallel.For(
                0,
                numberOfValues,
                i =>
                {
                    double x = startX + i * interval; // x increments by the specified interval
                    double open = Formula.CalculateFormula(x, mS) ?? 0;
                    double close = Formula.CalculateFormula(x + interval, mS) ?? 0;
                    double high = GetHighValue(x, interval, mS, 20); // Use fewer iterations for speed
                    double low = GetLowValue(x, interval, mS, 20); // Use fewer iterations for speed
                    DateTime date = Converter.ConvertDigitToDate(x);
                    double volume = Math.Round(
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
            double highValue = double.MinValue;
            double step = interval / samplePoints;

            for (int i = 0; i <= samplePoints; i++)
            {
                double result = Formula.CalculateFormula(x + step * i, mS) ?? 0;
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
            double lowValue = double.MaxValue;
            double step = interval / samplePoints;

            for (int i = 0; i <= samplePoints; i++)
            {
                double result = Formula.CalculateFormula(x + step * i, mS) ?? 0;
                if (result < lowValue)
                    lowValue = result;
            }

            return lowValue;
        }

        public static string[] GetStockNames(StockDALinterface stockDAL)
        {
            return stockDAL.GetStockNames();
        }
    }
}
