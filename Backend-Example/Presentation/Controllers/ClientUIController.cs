using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;
using DAL.BDaccess;

namespace Backend_Example.Controllers
{
    public static class ClientUIController
    {
        public static void ClientUIcontroller(this WebApplication app)
        {
            app.MapGet(
                    "/stocknames",
                    () =>
                    {
                        StockDAL stockDAL = new StockDAL();

                        return CandleStock.GetStockNames(stockDAL);
                    }
                )
                .WithName("GetStockNames")
                .WithOpenApi();
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
            app.Map(
                    "/stockWS",
                    async (HttpContext context) =>
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            await ProvideStock(webSocket);
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

        private static async Task ProvideStock(WebSocket webSocket)
        {
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
                    string stock = message[0];
                    double interval = double.Parse(message[1]);
                    double startX = double.Parse(message[2]);
                    double endX = double.Parse(message[3].Replace("\"", ""));
                    double mS = Converter.ConvertWordToNumber(stock) + 1;

                    double[] results = lineStock.GetValues(mS, startX, endX, interval);
                    string resultJson = JsonSerializer.Serialize(results);

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
                    CandleItem[] results = CandleStock.GetCandleValues(
                        stock,
                        startX,
                        endX,
                        interval,
                        new StockDAL()
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
