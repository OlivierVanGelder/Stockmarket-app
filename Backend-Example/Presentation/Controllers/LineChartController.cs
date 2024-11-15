using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend_Example.Data.BDaccess;
using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;
using DAL.BDaccess;
using Logic.Interfaces;

namespace Backend_Example.Controllers
{
    public static class LineChartController
    {
        public static void GetLineStock(this WebApplication app)
        {
            app.MapGet(
                    "/lineStock",
                    (string ticker, double interval, double start, double end) =>
                    {
                        LineStock stock = new();
                        double mS = Converter.ConvertWordToNumber(ticker) + 1;
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
