using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;
using DAL.BDaccess;

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
                        CandleItem[] results = CandleStock.GetCandleValues(
                            ticker,
                            start,
                            end,
                            interval,
                            new StockDAL()
                        );

                        return results;
                    }
                )
                .WithName("GetCandleStockFromTicker")
                .WithOpenApi();
        }
    }
}
