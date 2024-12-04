using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend_Example.Data.BDaccess;
using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;
using DAL.BDaccess;
using Logic.Interfaces;

namespace Backend_Example.Controllers
{
    public static class StockController
    {
        public static void Stockcontroller(this WebApplication app, IConfiguration configuration)
        {
            var stocks = app.MapGroup("/stocks").WithTags("Stocks");

            stocks
                .MapGet(
                    "/names",
                    async (IStockDAL stockDAL) =>
                    {
                        return CandleStock.GetStockNames(stockDAL);
                    }
                )
                .WithName("GetStockNames")
                .WithOpenApi();

            stocks
                .MapGet(
                    "/{ticker}",
                    async (
                        string ticker,
                        string type,
                        double interval,
                        double start,
                        double end,
                        IStockDAL stockDAL
                    ) =>
                    {
                        if (ticker == null)
                        {
                            return null;
                        }

                        if (type == "line")
                        {
                            LineStock stock = new();
                            DateTime startDate = Converter.ConvertDigitToDate(start);
                            DateTime endDate = Converter.ConvertDigitToDate(end);
                            TimeSpan intervalSpan = TimeSpan.FromDays(interval);

                            LineItem[] results = await stock.GetValues(
                                ticker,
                                startDate,
                                endDate,
                                intervalSpan,
                                stockDAL
                            );

                            return Results.Json(results);
                        }
                        else if (type == "candle ")
                        {
                            CandleItem[] results = await CandleStock.GetCandleValues(
                                ticker,
                                start,
                                end,
                                interval,
                                stockDAL
                            );

                            return Results.Json(results);
                        }
                        return null;
                    }
                )
                .WithName("GetStockFromTicker")
                .WithOpenApi();

            stocks
                .Map(
                    "/stockWS",
                    async (HttpContext context, DbStockEngine dbContext) =>
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            await ProvideStock(webSocket, dbContext);
                            return Results.Ok();
                        }
                        else
                        {
                            return Results.BadRequest();
                        }
                    }
                )
                .WithName("StockWS")
                .WithOpenApi();
        }

        private static async Task ProvideStock(WebSocket webSocket, DbStockEngine dbContext)
        {
            var stockDAL = new StockDAL(dbContext);

            var buffer = new byte[1024];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            while (!receiveResult.CloseStatus.HasValue)
            {
                string[] message = Encoding.UTF8.GetString(buffer).Split('-');
                if (message.Length == 4)
                {
                    LineStock lineStock = new();
                    string stock = message[0].Replace("\"", "");
                    DateTime startDate = Converter.ConvertDigitToDate(double.Parse(message[2]));
                    DateTime endDate = Converter.ConvertDigitToDate(
                        double.Parse(message[3].Replace("\"", ""))
                    );
                    TimeSpan intervalSpan = TimeSpan.FromDays(double.Parse(message[1]));
                    string resultJson = JsonSerializer.Serialize(
                        await lineStock.GetValues(stock, startDate, endDate, intervalSpan, stockDAL)
                    );
                    byte[] resultBuffer = Encoding.UTF8.GetBytes(resultJson);

                    await webSocket.SendAsync(
                        new ArraySegment<byte>(resultBuffer),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None
                    );
                }
                else if (message.Length == 5)
                {
                    string stock = message[0].Replace("\"", "");
                    double interval = double.Parse(message[1]);
                    double startX = double.Parse(message[2]);
                    double endX = double.Parse(message[3].Replace("\"", ""));
                    CandleItem[] results = await CandleStock.GetCandleValues(
                        stock,
                        startX,
                        endX,
                        interval,
                        stockDAL
                    );

                    string resultJson = JsonSerializer.Serialize(results);
                    byte[] resultBuffer = Encoding.UTF8.GetBytes(resultJson);

                    await webSocket.SendAsync(
                        new ArraySegment<byte>(resultBuffer),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None
                    );
                }

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None
            );
        }
    }
}
