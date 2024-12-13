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
    public static class StockController
    {
        public static void Stockcontroller(this WebApplication app, IConfiguration configuration)
        {
            var stocks = app.MapGroup("/stocks").WithTags("Stocks");

            stocks
                .MapGet(
                    "/names",
                    (IStockDAL stockDal) =>
                    {
                        return CandleStock.GetStockNames(stockDal);
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
                        IStockDAL stockDal
                    ) =>
                    {
                        if (ticker == "")
                        {
                            return null;
                        }

                        switch (type)
                        {
                            case "line":
                            {
                                LineStock stock = new();
                                var startDate = Converter.ConvertDigitToDate(start);
                                var endDate = Converter.ConvertDigitToDate(end);
                                var intervalSpan = TimeSpan.FromDays(interval);

                                var results = await stock.GetValues(
                                    ticker,
                                    startDate,
                                    endDate,
                                    intervalSpan,
                                    stockDal
                                );

                                return Results.Json(results);
                            }
                            case "Candle":
                            {
                                var results = await CandleStock.GetCandleValues(
                                    ticker,
                                    start,
                                    end,
                                    interval,
                                    stockDal
                                );

                                return Results.Json(results);
                            }
                            default: return null;
                        }
                    }
                )
                .WithName("GetStockFromTicker")
                .WithOpenApi();

            stocks
                .Map(
                    "/StockWS",
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
            var stockDal = new StockDAL(dbContext);

            var buffer = new byte[1024];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            while (!receiveResult.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer).Split('-');
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
                        await lineStock.GetValues(stock, startDate, endDate, intervalSpan, stockDal)
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
                        stockDal
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
