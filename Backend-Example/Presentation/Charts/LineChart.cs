using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;

namespace Backend_Example.Presentation.Charts
{
    public static class LineChart
    {
        public static void GetLineStock(this WebApplication app)
        {
            app.MapGet(
                    "/stock",
                    (string ticker, double interval, double start, double end) =>
                    {
                        LineStock stock = new();
                        Ticker tickerConverter = new();
                        double mS = tickerConverter.ConvertWordToNumber(ticker) + 1;
                        double startX = start;
                        double endX = end;

                        double[] results = stock.GetValues(mS, startX, endX, interval);

                        return results;
                    }
                )
                .WithName("GetStockFromTicker")
                .WithOpenApi();
        }
    }
}
