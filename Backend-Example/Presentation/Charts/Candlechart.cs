using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;

namespace Backend_Example.Charts
{
    public static class Candlechart
    {
        public static void GetCandleStock(this WebApplication app)
        {
            app.MapGet(
                    "/candlestock",
                    (string ticker, double interval, double start, double end) =>
                    {
                        CandleStock stock = new();
                        Ticker tickerConverter = new();
                        double mS = tickerConverter.ConvertWordToNumber(ticker) + 1;
                        double startX = start;
                        double endX = end;

                        CandleItem[] results = CandleStock.GetCandleValues(
                            mS,
                            startX,
                            endX,
                            interval
                        );

                        return results;
                    }
                )
                .WithName("GetCandleStockFromTicker")
                .WithOpenApi();
        }
    }
}
