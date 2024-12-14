using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Logic.Models;
using Logic.Stocks;
using DAL.DbAccess;
using Logic.Interfaces;

namespace Backend_Example.Controllers;

public static class StockController
{
    public static void NewStockController(this WebApplication app)
    {
        var stocks = app.MapGroup("/stocks").WithTags("Stocks");

        stocks
            .MapGet(
                "/names",
                (IStockDAal stockDal) =>
                {
                    return () => CandleStock.GetStockNames(stockDal);
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
                    IStockDAal stockDal
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
                            var startDate = Converter.ConvertDigitToDate(start);
                            var endDate = Converter.ConvertDigitToDate(end);
                            var intervalSpan = TimeSpan.FromDays(interval);

                            var results = await LineStock.GetValues(
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
                    if (!context.WebSockets.IsWebSocketRequest)
                    {
                        return Results.BadRequest();
                    }
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await ProvideStock(webSocket, dbContext);
                    return Results.Ok();
                }
            )
            .WithName("StockWS")
            .WithOpenApi();
    }

    private static async Task ProvideStock(WebSocket webSocket, DbStockEngine dbContext)
    {
        var stockDal = new StockDal(dbContext);

        var buffer = new byte[1024];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None
        );

        while (!receiveResult.CloseStatus.HasValue)
        {
            var message = Encoding.UTF8.GetString(buffer).Split('-');
            switch (message.Length)
            {
                case 4:
                {
                    var stock = message[0].Replace("\"", "");
                    var startDate = Converter.ConvertDigitToDate(double.Parse(message[2]));
                    var endDate = Converter.ConvertDigitToDate(
                        double.Parse(message[3].Replace("\"", ""))
                    );
                    var intervalSpan = TimeSpan.FromDays(double.Parse(message[1]));
                    var resultJson = JsonSerializer.Serialize(
                        await LineStock.GetValues(stock, startDate, endDate, intervalSpan, stockDal)
                    );
                    var resultBuffer = Encoding.UTF8.GetBytes(resultJson);

                    await webSocket.SendAsync(
                        new ArraySegment<byte>(resultBuffer),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None
                    );
                    break;
                }
                case 5:
                {
                    var stock = message[0].Replace("\"", "");
                    var interval = double.Parse(message[1]);
                    var startX = double.Parse(message[2]);
                    var endX = double.Parse(message[3].Replace("\"", ""));
                    var results = await CandleStock.GetCandleValues(
                        stock,
                        startX,
                        endX,
                        interval,
                        stockDal
                    );

                    var resultJson = JsonSerializer.Serialize(results);
                    var resultBuffer = Encoding.UTF8.GetBytes(resultJson);

                    await webSocket.SendAsync(
                        new ArraySegment<byte>(resultBuffer),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None
                    );
                    break;
                }
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
