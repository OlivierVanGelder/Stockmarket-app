using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend_Example.Data.BDaccess;
using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;
using Backend_Example.Models;
using DAL.BDaccess;
using DAL.Tables;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;

public static class ClientUIController
{
    public static void ClientUIcontroller(this WebApplication app, IConfiguration configuration)
    {
        app.MapGet(
                "/stocks/names",
                async (StockDALinterface stockDAL) =>
                {
                    return CandleStock.GetStockNames(stockDAL);
                }
            )
            .WithName("GetStockNames")
            .WithOpenApi();

        app.MapGet(
                "/lineStock",
                async (
                    string ticker,
                    double interval,
                    double start,
                    double end,
                    StockDALinterface stockDAL
                ) =>
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
                        await ProvideStock(webSocket, app.Services);
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
                async (
                    string ticker,
                    double interval,
                    double start,
                    double end,
                    StockDALinterface stockDAL
                ) =>
                {
                    CandleItem[] results = await CandleStock.GetCandleValues(
                        ticker,
                        start,
                        end,
                        interval,
                        stockDAL
                    );

                    return results;
                }
            )
            .WithName("GetCandleStockFromTicker")
            .WithOpenApi();

        app.MapPost(
            "/accounts/login/verify",
            async (
                HttpContext context,
                DbStockEngine dbContext,
                UserManager<IdentityUser> userManager
            ) =>
            {
                var loginRequest = await context.Request.ReadFromJsonAsync<LoginRequest>();
                if (loginRequest == null)
                    return Results.BadRequest();

                UserDAL userDAL = new(dbContext, userManager);
                bool user = await userDAL.VerifyUser(loginRequest.Name, loginRequest.Password);

                if (!user)
                    return Results.Unauthorized();

                string secretKey = configuration["SecretKey:Key"];

                var token = Logic.Functions.JwtHelper.GenerateToken(loginRequest.Name, secretKey);
                return Results.Json(new { Token = token });
            }
        );
        app.MapPost(
            "/accounts/login/register",
            async (
                HttpContext context,
                DbStockEngine dbContext,
                UserManager<IdentityUser> userManager
            ) =>
            {
                var registerRequest = await context.Request.ReadFromJsonAsync<RegisterRequest>();
                if (registerRequest == null)
                    return Results.BadRequest();

                UserDAL userDAL = new(dbContext, userManager);
                bool user = await userDAL.AddUserAsync(
                    registerRequest.Name,
                    registerRequest.Password
                );

                string secretKey = configuration["SecretKey:Key"];
                var token = Logic.Functions.JwtHelper.GenerateToken(
                    registerRequest.Name,
                    secretKey
                );
                return Results.Json(new { Token = token });
            }
        );
    }

    private static async Task ProvideStock(WebSocket webSocket, IServiceProvider serviceProvider)
    {
        var stockDAL = serviceProvider.GetRequiredService<StockDALinterface>();

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
