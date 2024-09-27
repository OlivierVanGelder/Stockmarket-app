namespace Backend_Example
{
    public static class Stock
    {
        public static void SetupStock(this WebApplication app)
        {

            app.MapGet("/stock", (string ticker, double interval, double start, double end) =>
            {

                double mS = ConvertWordToNumber(ticker) + 1;
                double startX = start;
                double endX = end;

                double[] results = ComputeValues(mS, startX, endX, interval);

                return results;
            })
            .WithName("GetStockFromTicker")
            .WithOpenApi();
        }

        private static double[] ComputeValues(double mS, double startX, double endX, double interval)
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


        private static double CalculateFormula(double x, double mS)
        {
            double lD = mS % 10 + 1;
            double term1 = Math.Cos(0.345 * x) * Math.Sin(1 / ((mS + 1) / 552) * x) * Math.Pow(Math.Sin(0.1134 * x), 2) * 0.2;
            double term2 = Math.Abs(Math.Pow(Math.Cos(0.356 * x), 3));
            double term3 = Math.Abs(Math.Sin((1.0 / ((mS + 1) / 1.23)) * x) * (mS / 30)) + Math.Sin(0.876 * x) + Math.Cos(1320*x) * (0.15165 + lD/70 ) + Math.Cos(820 * x) * (0.151644 + lD / 50 ) + Math.Cos((620+ mS * 2) * x) * 0.1134 + Math.Cos((260 + lD*5) * x) * 0.09456;
            double term4 = Math.Sin((1.0 / ((mS + 1) / 42) * x) * 1.4) + Math.Cos(10*x) * 0.25 + Math.Cos(100 * x) * 0.063 + Math.Sin(13 * x) * 0.39;
            double term5 = Math.Sin(0.00229 * x) * lD / 35 + Math.Cos(0.003343 * x) * lD / 23 + Math.Sin(0.00105 * x) * lD / 31 + Math.Cos(0.002343 * x) * lD / 19;
            double term6 = Math.Cos(0.00291 * x) * 21 + Math.Cos(0.0012 * x) * (10.654 + lD) + Math.Cos(0.000826 * x) * (30 - mS/10) + Math.Cos(0.001176 * x) * (10 + mS/10);
            double term7 = (mS / 1589.0) * x;
            double term8 = Math.Sin((0.19 + mS / 300) * x) * lD * -0.9 + Math.Cos((0.07343 + mS / 500) * x) * mS / 15 + Math.Sin((0.105 + mS / 1000) * x) * lD + Math.Cos((0.02343 + mS / 2000) * x) * lD;
            return term1 + term2 + term3 + term4 + term5 + term6 + term7 + term8 + (0.3 * mS);
        }

        public static int ConvertWordToNumber(string word)
        {
            word = word + "AAAA";
            word.Substring(0, 4);
            return word.ToUpper().Sum(c => c - 'A');
        }
    }
}