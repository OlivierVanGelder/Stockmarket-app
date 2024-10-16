using Backend_Example.Logic.Classes;

namespace Backend_Example.Logic.Stocks
{
    public class LineStock
    {
        public double[] GetValues(double mS, double startX, double endX, double interval)
        {
            // Calculate the number of values based on the range and interval
            int numberOfValues = (int)((endX - startX) / interval) + 1;
            double[] values = new double[numberOfValues];
            Formula formula = new Formula();

            for (int i = 0; i < numberOfValues; i++)
            {
                double x = startX + i * interval; // x increments by the specified interval
                double? outcome = Formula.CalculateFormula(x, mS);
                double result = outcome ?? 0;
                values[i] = Math.Round(result, 2); // Round the result to two decimals
            }

            return values;
        }
    }
}
