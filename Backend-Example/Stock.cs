namespace Backend_Example
{
    public static class Stock
    {
        public static void SetupStock(this WebApplication app)
        {
            app.MapGet(
                    "/stock",
                    (string ticker, double interval, double start, double end) =>
                    {
                        double mS = ConvertWordToNumber(ticker) + 1;
                        double startX = start;
                        double endX = end;

                        double[] results = GetValues(mS, startX, endX, interval);

                        return results;
                    }
                )
                .WithName("GetStockFromTicker")
                .WithOpenApi();
        }

        public static void GetCandleStock(this WebApplication app)
        {
            app.MapGet(
                    "/candlestock",
                    (string ticker, double interval, double start, double end) =>
                    {
                        double mS = ConvertWordToNumber(ticker) + 1;
                        double startX = start;
                        double endX = end;

                        CandleItem[] results = GetCandleValues(mS, startX, endX, interval);

                        return results;
                    }
                )
                .WithName("GetCandleStockFromTicker")
                .WithOpenApi();
        }

        private static double[] GetValues(double mS, double startX, double endX, double interval)
        {
            // Calculate the number of values based on the range and interval
            int numberOfValues = (int)((endX - startX) / interval) + 1;
            double[] values = new double[numberOfValues];

            for (int i = 0; i < numberOfValues; i++)
            {
                double x = startX + i * interval; // x increments by the specified interval
                double result = CalculateFormula(x, mS);
                values[i] = Math.Round(result, 2); // Round the result to two decimals
            }

            return values;
        }

        private static CandleItem[] GetCandleValues(
            double mS,
            double startX,
            double endX,
            double interval
        )
        {
            // Calculate the number of values based on the range and interval
            int numberOfValues = (int)((endX - startX) / interval) + 1;
            CandleItem[] values = new CandleItem[numberOfValues];

            for (int i = 0; i < numberOfValues; i++)
            {
                double x = startX + i * interval; // x increments by the specified interval
                double open = CalculateFormula(x, mS);
                double close = CalculateFormula(x + interval, mS);
                double high = open;
                double low = open;
                for (int j = 0; j < 20; j++)
                {
                    double temp = CalculateFormula(x + (0.05 * j * interval), mS);
                    if (temp > high)
                    {
                        high = temp;
                    }
                    else if (temp < low)
                    {
                        low = temp;
                    }
                }
                open = Math.Round(open, 2);
                close = Math.Round(close, 2);
                high = Math.Round(high, 2);
                low = Math.Round(low, 2);
                values[i] = new CandleItem(open, close, high, low);
            }
            return values;
        }

        private static double CalculateFormula(double x, double mS)
        {
            /*Define the last digit*/double lD = mS % 10 + 1;
            /**/double term1 =
                Math.Cos(0.345 * x)
                * Math.Sin(1 / ((mS + 1) / 52) * x)
                * Math.Pow(Math.Sin(0.1134 * x), 2)
                * 0.2;
            double term2 = Math.Abs(Math.Pow(Math.Cos(0.356 * x), 3));
            double term3 =
                Math.Abs(Math.Sin((1.0 / ((mS + 1) / 1.23)) * x) * (mS / 30))
                + Math.Sin(176 * x) * 0.345
                + Math.Cos(1320 * x) * (0.15165 + lD / 70)
                + Math.Cos(820 * x) * (0.0461644 + lD / 50)
                + Math.Cos((620 + mS * 2) * x) * 0.1134
                + Math.Cos((260 + lD * 5) * x) * 0.09456
                + Math.Sin(1760 * x) * 0.0345
                + Math.Cos(13200 * x) * (0.015165 + lD / 69)
                + Math.Cos(8200 * x) * (0.0461644 + lD / 93)
                + Math.Cos((620 + mS * 20) * x) * 0.01134
                + Math.Cos((260 + lD * 50) * x) * 0.09456;
            /*Multiple waves a day*/
            double term4 =
                Math.Sin((1.0 / ((mS + 1) / 42) * x) * 1.4)
                + Math.Cos(10.94 * x) * 0.75
                + Math.Cos(15.43 * x) * 0.435
                + Math.Cos(100 * x) * 0.193
                + Math.Sin(23 * x) * 0.5275;
            double term5 =
                Math.Sin(0.00229 * x) * (lD + 1) / 85
                + Math.Cos(0.003343 * x) * (lD + 1) / 13
                + Math.Sin(0.00505 * x) * (lD + 1) / 19
                + Math.Cos(0.00343 * x) * (lD + 1) / 63;
            double term6 =
                Math.Cos(0.00291 * x) * 21
                + Math.Cos(0.0012 * x) * (10.654 + lD)
                + Math.Cos(0.000826 * x) * (30 - mS / 10)
                + Math.Cos(0.001176 * x) * (10 + mS / 10)
                + Math.Abs(Math.Cos(1.3438 * x) * 1.254)
                + Math.Abs(Math.Cos(1.9438 * x) * 2.154)
                + Math.Abs(Math.Cos(1.5438 * x) * 1.054)
                + Math.Abs(Math.Cos(1.2438 * x) * 1.954)
                + Math.Abs(Math.Cos(1.0438 * x) * 2.654);
            double term7 = (mS / 1589.0) * x;
            double term8 =
                Math.Sin((0.19 + mS / 300) * x) * lD * -0.9
                + Math.Cos((0.07343 + mS / 500) * x) * mS / 15
                + Math.Sin((0.105 + mS / 1000) * x) * lD
                + Math.Cos((0.02343 + mS / 2000) * x) * lD;
            return term1 + term2 + term3 + term4 + term5 + term6 + term7 + term8 + (0.3 * mS);
        }

        public static int ConvertWordToNumber(string word)
        {
            word = word + "AAAA";
            word.Substring(0, 4);
            return word.ToUpper().Sum(c => c - 'A');
        }

        private class CandleItem
        {
            public double Open { get; }
            public double Close { get; }
            public double High { get; }
            public double Low { get; }

            public CandleItem(double open, double close, double high, double low)
            {
                Open = open;
                Close = close;
                High = high;
                Low = low;
            }
        }
    }
}
