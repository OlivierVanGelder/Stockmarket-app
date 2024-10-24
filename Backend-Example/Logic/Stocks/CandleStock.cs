﻿using Backend_Example.Logic.Classes;
using Logic.Interfaces;

namespace Backend_Example.Logic.Stocks
{
    public class CandleStock
    {
        public static CandleItem[] GetCandleValues(
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
                double open = Formula.CalculateFormula(x, mS) ?? 0;
                double close = Formula.CalculateFormula(x + interval, mS) ?? 0;
                double high = open;
                double low = open;

                for (int j = 0; j < 32; j++)
                {
                    double temp = Formula.CalculateFormula(x + 0.03125 * j * interval, mS) ?? 0;
                    if (temp > high)
                    {
                        high = temp;
                    }
                    else
                    {
                        low = temp;
                    }
                }
                double volume = Math.Round(
                    100 / open * (high - low) * (2582 + (Math.Sin(30000 * x) + 1) * mS * 50),
                    3
                );
                open = Math.Round(open, 2);
                close = Math.Round(close, 2);
                high = Math.Round(high, 2);
                low = Math.Round(low, 2);
                values[i] = new CandleItem(open, close, high, low, volume);
            }
            return values;
        }

        public static string[] GetStockNames(StockDALinterface stockDAL)
        {
            return stockDAL.GetStockNames();
        }
    }
}
