using System.Data.Entity;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend_Example.Data.BDaccess;
using Backend_Example.Models;
using Logic.Functions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Backend_Example.Controllers
{
    public static class UserController
    {
        public static void Usercontroller(this WebApplication app, IConfiguration configuration)
        {
            app.MapPost(
                "/accounts/login/verify",
                async (
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    var loginRequest = await context.Request.ReadFromJsonAsync<LoginRequest>();
                    if (loginRequest == null)
                        return Results.BadRequest();

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    bool user = await userDAL.VerifyUser(loginRequest.Name, loginRequest.Password);

                    if (!user)
                        return Results.Unauthorized();

                    string userId = await userDAL.GetUserId(loginRequest.Name);

                    string secretKey = configuration["SecretKey:Key"];

                    var token = JwtHelper.GenerateToken(loginRequest.Name, secretKey);
                    return Results.Json(new { token, userId });
                }
            );
            app.MapPost(
                "/accounts/login/register",
                async (
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    var registerRequest =
                        await context.Request.ReadFromJsonAsync<RegisterRequest>();
                    if (registerRequest == null)
                        return Results.BadRequest();

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    bool user = await userDAL.AddUserAsync(
                        registerRequest.Name,
                        registerRequest.Password
                    );

                    string secretKey = configuration["SecretKey:Key"];
                    var token = JwtHelper.GenerateToken(registerRequest.Name, secretKey);
                    return Results.Json(new { Token = token });
                }
            );
            app.MapPost(
                "/accounts/user/balance",
                async (
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    string userID = await context.Request.ReadFromJsonAsync<string>();
                    if (userID == null)
                        return Results.BadRequest();

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    User user = new();
                    double userBalance = await user.GetUserBalance(userDAL, userID);

                    return Results.Json(new { UserBalance = userBalance });
                }
            );
            app.MapPost(
                "/accounts/user/name",
                async (
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    string userID = await context.Request.ReadFromJsonAsync<string>();
                    if (userID == null)
                        return Results.BadRequest();

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    User user = new();
                    string userName = await user.GetUserName(userDAL, userID);

                    return Results.Json(new { UserName = userName });
                }
            );
            app.MapPost(
                "/accounts/stock/buy",
                async (
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    var stockTradeRequest =
                        await context.Request.ReadFromJsonAsync<StockTradeRequest>();
                    if (stockTradeRequest == null)
                        return Results.BadRequest("Invalid request payload.");

                    string? userID = stockTradeRequest.UserID;
                    int? amount = stockTradeRequest.Amount;
                    string? ticker = stockTradeRequest.Ticker;
                    double? price = stockTradeRequest.Price;

                    if (
                        amount == null
                        || ticker == null
                        || string.IsNullOrEmpty(userID)
                        || string.IsNullOrEmpty(ticker)
                        || amount <= 0
                    )
                        return Results.BadRequest("Missing or invalid data.");

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    User user = new();
                    bool success = await user.BuyUserStock(
                        userDAL,
                        userID,
                        ticker,
                        amount ?? 0,
                        price ?? 0
                    );

                    return Results.Json(new { success });
                }
            );

            app.MapPost(
                "/accounts/stock/sell",
                async (
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    var stockTradeRequest =
                        await context.Request.ReadFromJsonAsync<StockTradeRequest>();
                    if (stockTradeRequest == null)
                        return Results.BadRequest("Invalid request payload.");

                    string? userID = stockTradeRequest.UserID;
                    int? amount = stockTradeRequest.Amount;
                    string? ticker = stockTradeRequest.Ticker;
                    double? price = stockTradeRequest.Price;

                    if (
                        amount == null
                        || ticker == null
                        || string.IsNullOrEmpty(userID)
                        || string.IsNullOrEmpty(ticker)
                        || amount <= 0
                    )
                        return Results.BadRequest("Missing or invalid data.");

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    User user = new();
                    bool success = await user.SellUserStock(
                        userDAL,
                        userID,
                        ticker,
                        amount ?? 0,
                        price ?? 0
                    );

                    return Results.Json(new { success });
                }
            );

            app.MapGet(
                "/accounts/stock/amount",
                async (
                    string userID,
                    string ticker,
                    IUserDAL userDAL
                ) =>
                {
                    User user = new();
                    double result = await user.GetUserStockAmount(userDAL, userID, ticker);

                    return Results.Json(result);
                }
            )
            .WithName("GetStockAmount")
            .WithOpenApi();
        }
    }
}
