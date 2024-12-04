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
            var users = app.MapGroup("/users").WithTags("Users");

            users.MapGet(
                "/{id}/login",
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
            users.MapPost(
                "/",
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
            users.MapGet(
                "/{id}/balance",
                async (
                    string id,
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    if (id == null)
                        return Results.BadRequest();

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    User user = new();
                    double userBalance = await user.GetUserBalance(userDAL, id);

                    return Results.Json(new { UserBalance = userBalance });
                }
            );
            users.MapGet(
                "/{id}/name",
                async (
                    string id,
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    if (id == null)
                        return Results.BadRequest();

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    User user = new();
                    string userName = await user.GetUserName(userDAL, id);

                    return Results.Json(new { UserName = userName });
                }
            );

            users.MapPut(
                "/{id}/stock",
                async (
                    string id,
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    var stockTradeRequest =
                        await context.Request.ReadFromJsonAsync<StockTradeRequest>();

                    if (stockTradeRequest == null)
                        return Results.BadRequest("Invalid request payload.");

                    int? amount = stockTradeRequest.Amount;
                    string? ticker = stockTradeRequest.Ticker;
                    double? price = stockTradeRequest.Price;
                    string? action = stockTradeRequest.Action?.ToLower(); // buy or sell

                    if (
                        string.IsNullOrEmpty(id)
                        || string.IsNullOrEmpty(ticker)
                        || string.IsNullOrEmpty(action)
                        || amount == null
                        || amount <= 0
                    )
                        return Results.BadRequest("Missing or invalid data.");

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    User user = new();
                    bool success = false;

                    switch (action)
                    {
                        case "buy":
                            success = await user.BuyUserStock(
                                userDAL,
                                id,
                                ticker,
                                amount.Value,
                                price ?? 0
                            );
                            break;
                        case "sell":
                            success = await user.SellUserStock(
                                userDAL,
                                id,
                                ticker,
                                amount.Value,
                                price ?? 0
                            );
                            break;
                        default:
                            return Results.BadRequest("Invalid action. Must be 'buy' or 'sell'.");
                    }

                    return Results.Json(new { success });
                }
            );

            users
                .MapGet(
                    "/{id}/stock/amount",
                    async (string id, string ticker, IUserDAL userDAL) =>
                    {
                        User user = new();
                        double result = await user.GetUserStockAmount(userDAL, id, ticker);

                        return Results.Json(result);
                    }
                )
                .WithName("GetStockAmount")
                .WithOpenApi();
        }
    }
}
