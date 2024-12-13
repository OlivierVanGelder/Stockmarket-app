using DAL.DbAccess;
using Backend_Example.Models;
using Logic.Functions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Backend_Example.Controllers
{
    [Authorize]
    public static class UserController
    {
        public static void Usercontroller(this WebApplication app, IConfiguration configuration)
        {
            var users = app.MapGroup("/users").WithTags("Users");

            users.MapGet(
                "/{username}/login",
                async (
                    string username,
                    string password,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    if (password == "" || username == "")
                        return Results.BadRequest();

                    UserDal userDal = new(dbContext, userManager);
                    bool user = await userDal.VerifyUser(username, password);

                    if (!user)
                        return Results.Unauthorized();

                    var userId = await userDal.GetUserId(username);

                    var secretKey = configuration["Jwt:Key"] ?? "";

                    var token = JwtHelper.GenerateToken(username, userId, secretKey);
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

                    UserDal userDal = new(dbContext, userManager);
                    var newUser = await User.AddUser(
                        userDal,
                        registerRequest.Name,
                        registerRequest.Password
                    );

                    if (!newUser)
                        return Results.Conflict();
                    var userId = await userDal.GetUserId(registerRequest.Name);
                    var secretKey = configuration["Jwt:Key"];
                    var token = JwtHelper.GenerateToken(registerRequest.Name, userId, secretKey ?? "");
                    return Results.Json(new { Token = token, UserId = userId });
                }
            );

            users
                .MapDelete(
                    "/{userId}",
                    async (
                        string userId,
                        HttpContext context,
                        DbStockEngine dbContext,
                        UserManager<DAL.Tables.User> userManager
                    ) =>
                    {
                        if (UserAuthorization(userId, context) != Results.Ok())
                        {
                            return UserAuthorization(userId, context);
                        }

                        string? userIdFromToken = context.User.FindFirst("userId")?.Value;

                        if (string.IsNullOrEmpty(userIdFromToken))
                            return Results.Unauthorized();

                        if (userIdFromToken != userId)
                            return Results.Forbid();

                        UserDal userDal = new(dbContext, userManager);
                        var success = await userDal.DeleteUser(userId);
                        if (!success)
                        {
                            return Results.Conflict();   
                        }
                        return Results.Ok();
                    }
                )
                .RequireAuthorization();

            users
                .MapGet(
                    "/{userId}/balance",
                    async (
                        string userId,
                        HttpContext context,
                        DbStockEngine dbContext,
                        UserManager<DAL.Tables.User> userManager
                    ) =>
                    {
                        if (UserAuthorization(userId, context) != Results.Ok())
                        {
                            return UserAuthorization(userId, context);
                        }

                        if (userId == "")
                            return Results.BadRequest();

                        var userIdFromToken = context.User.FindFirst("userId")?.Value;

                        if (string.IsNullOrEmpty(userIdFromToken))
                            return Results.Unauthorized();

                        if (userIdFromToken != userId)
                            return Results.Forbid();

                        UserDal userDal = new(dbContext, userManager);
                        var userBalance = await User.GetUserBalance(userDal, userId);

                        return Results.Json(new { UserBalance = userBalance });
                    }
                )
                .RequireAuthorization();

            users
                .MapGet(
                    "/{id}/name",
                    async (
                        string id,
                        HttpContext context,
                        DbStockEngine dbContext,
                        UserManager<DAL.Tables.User> userManager
                    ) =>
                    {
                        if (UserAuthorization(id, context) != Results.Ok())
                        {
                            return UserAuthorization(id, context);
                        }

                        if (id == "")
                            return Results.BadRequest();

                        UserDal userDal = new(dbContext, userManager);
                        var userName = await User.GetUserName(userDal, id);

                        return Results.Json(new { UserName = userName });
                    }
                )
                .RequireAuthorization();

            users
                .MapPut(
                    "/{id}/stock",
                    async (
                        string id,
                        HttpContext context,
                        DbStockEngine dbContext,
                        UserManager<DAL.Tables.User> userManager
                    ) =>
                    {
                        if (UserAuthorization(id, context) != Results.Ok())
                        {
                            return UserAuthorization(id, context);
                        }

                        var stockTradeRequest =
                            await context.Request.ReadFromJsonAsync<StockTradeRequest>();

                        if (stockTradeRequest == null)
                            return Results.BadRequest("Invalid request payload.");

                        var amount = stockTradeRequest.Amount;
                        var ticker = stockTradeRequest.Ticker;
                        var price = stockTradeRequest.Price;
                        var action = stockTradeRequest.Action?.ToLower();

                        if (
                            string.IsNullOrEmpty(id)
                            || string.IsNullOrEmpty(ticker)
                            || string.IsNullOrEmpty(action)
                            || amount == null
                            || amount <= 0
                        )
                            return Results.BadRequest("Missing or invalid data.");

                        UserDal userDal = new(dbContext, userManager);
                        bool success;

                        switch (action)
                        {
                            case "buy":
                                success = await User.BuyUserStock(
                                    userDal,
                                    id,
                                    ticker,
                                    amount.Value,
                                    price ?? 0
                                );
                                break;
                            case "sell":
                                success = await User.SellUserStock(
                                    userDal,
                                    id,
                                    ticker,
                                    amount.Value,
                                    price ?? 0
                                );
                                break;
                            default:
                                return Results.BadRequest(
                                    "Invalid action. Must be 'buy' or 'sell'."
                                );
                        }

                        return Results.Json(new { success });
                    }
                )
                .RequireAuthorization();

            users
                .MapGet(
                    "/{id}/stock/amount",
                    async (string id, string ticker, IUserDal userDal, HttpContext context) =>
                    {
                        if (UserAuthorization(id, context) != Results.Ok())
                        {
                            return UserAuthorization(id, context);
                        }
                        
                        var result = await User.GetUserStockAmount(userDal, id, ticker);

                        return Results.Json(result);
                    }
                )
                .RequireAuthorization()
                .WithName("GetStockAmount")
                .WithOpenApi();
        }

        private static IResult UserAuthorization(string userId, HttpContext context)
        {
            if (userId == "")
                return Results.BadRequest();

            var userIdFromToken = context.User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
                return Results.Unauthorized();

            if(userIdFromToken != userId)  
                return Results.Forbid();
            return Results.Ok();
        }
    }
}
