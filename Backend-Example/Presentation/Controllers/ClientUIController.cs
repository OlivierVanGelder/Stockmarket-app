using System.Data.Entity;
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
            async (HttpContext context, DbStockEngine dbContext, UserManager<User> userManager) =>
            {
                var loginRequest = await context.Request.ReadFromJsonAsync<LoginRequest>();
                if (loginRequest == null)
                    return Results.BadRequest();

                UserDAL userDAL = new(dbContext, userManager);
                bool user = await userDAL.VerifyUser(loginRequest.Name, loginRequest.Password);

                if (!user)
                    return Results.Unauthorized();

                string userId = await userDAL.GetUserId(loginRequest.Name);

                string secretKey = configuration["SecretKey:Key"];

                var token = Logic.Functions.JwtHelper.GenerateToken(loginRequest.Name, secretKey);
                return Results.Json(new { token, userId });
            }
        );
        app.MapPost(
            "/accounts/login/register",
            async (HttpContext context, DbStockEngine dbContext, UserManager<User> userManager) =>
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
        app.MapPost(
            "/accounts/user/balance",
            async (HttpContext context, DbStockEngine dbContext, UserManager<User> userManager) =>
            {
                string userID = await context.Request.ReadFromJsonAsync<string>();
                if (userID == null)
                    return Results.BadRequest();

                UserDAL userDAL = new(dbContext, userManager);
                Logic.Functions.User user = new();
                double userBalance = await user.GetUserBalance(userDAL, userID);

                return Results.Json(new { UserBalance = userBalance });
            }
        );
        app.MapPost(
            "/accounts/user/name",
            async (HttpContext context, DbStockEngine dbContext, UserManager<User> userManager) =>
            {
                string userID = await context.Request.ReadFromJsonAsync<string>();
                if (userID == null)
                    return Results.BadRequest();

                UserDAL userDAL = new(dbContext, userManager);
                Logic.Functions.User user = new();
                string userName = await user.GetUserName(userDAL, userID);

                return Results.Json(new { UserName = userName });
            }
        );
        app.MapPost(
            "/accounts/stock/buy",
            async (HttpContext context, DbStockEngine dbContext, UserManager<User> userManager) =>
            {
                var stockTradeRequest = await context.Request.ReadFromJsonAsync<StockTradeRequest>();
                if (stockTradeRequest == null)
                    return Results.BadRequest("Invalid request payload.");

                string? userID = stockTradeRequest.UserID;
                int? amount = stockTradeRequest.Amount;
                string? ticker = stockTradeRequest.Ticker;
                double? price = stockTradeRequest.Price;

                if (amount == null || ticker == null || string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(ticker) || amount <= 0)
                    return Results.BadRequest("Missing or invalid data.");

                UserDAL userDAL = new(dbContext, userManager);
                Logic.Functions.User user = new();
                bool success = await user.BuyUserStock(userDAL, userID, ticker, amount ?? 0, price ?? 0);

                return Results.Json(new { success });
            }
        );

        app.MapPost(
            "/accounts/stock/sell",
            async (HttpContext context, DbStockEngine dbContext, UserManager<User> userManager) =>
            {
                var stockTradeRequest = await context.Request.ReadFromJsonAsync<StockTradeRequest>();
                if (stockTradeRequest == null)
                    return Results.BadRequest("Invalid request payload.");

                string? userID = stockTradeRequest.UserID;
                int? amount = stockTradeRequest.Amount;
                string? ticker = stockTradeRequest.Ticker;
                double? price = stockTradeRequest.Price;

                if (amount == null || ticker == null || string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(ticker) || amount <= 0)
                    return Results.BadRequest("Missing or invalid data.");

                UserDAL userDAL = new(dbContext, userManager);
                Logic.Functions.User user = new();
                bool success = await user.SellUserStock(userDAL, userID, ticker, amount ?? 0, price ?? 0);

                return Results.Json(new { success });
            }
        );
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
