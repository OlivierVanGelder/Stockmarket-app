﻿using Backend_Example.Data.BDaccess;
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
                    HttpContext context,
                    DbStockEngine dbContext,
                    UserManager<DAL.Tables.User> userManager
                ) =>
                {
                    if (password == null || username == null)
                        return Results.BadRequest();

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    bool user = await userDAL.VerifyUser(username, password);

                    if (!user)
                        return Results.Unauthorized();

                    string userId = await userDAL.GetUserId(username);

                    string secretKey = configuration["Jwt:Key"];

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

                    DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                    User user = new();
                    bool newUser = await user.AddUser(
                        userDAL,
                        registerRequest.Name,
                        registerRequest.Password
                    );

                    if (!newUser)
                        return Results.Conflict();
                    string userId = await userDAL.GetUserId(registerRequest.Name);
                    string secretKey = configuration["Jwt:Key"];
                    var token = JwtHelper.GenerateToken(registerRequest.Name, userId, secretKey);
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

                        DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                        bool succes = await userDAL.DeleteUser(userId);
                        if (!succes)
                            return Results.Conflict();
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

                        if (userId == null)
                            return Results.BadRequest();

                        string? userIdFromToken = context.User.FindFirst("userId")?.Value;

                        if (string.IsNullOrEmpty(userIdFromToken))
                            return Results.Unauthorized();

                        if (userIdFromToken != userId)
                            return Results.Forbid();

                        DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                        User user = new();
                        double userBalance = await user.GetUserBalance(userDAL, userId);

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

                        if (id == null)
                            return Results.BadRequest();

                        DAL.BDaccess.UserDAL userDAL = new(dbContext, userManager);
                        User user = new();
                        string userName = await user.GetUserName(userDAL, id);

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

                        int? amount = stockTradeRequest.Amount;
                        string? ticker = stockTradeRequest.Ticker;
                        double? price = stockTradeRequest.Price;
                        string? action = stockTradeRequest.Action?.ToLower();

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
                    async (string id, string ticker, IUserDAL userDAL, HttpContext context) =>
                    {
                        if (UserAuthorization(id, context) != Results.Ok())
                        {
                            return UserAuthorization(id, context);
                        }

                        User user = new();
                        double result = await user.GetUserStockAmount(userDAL, id, ticker);

                        return Results.Json(result);
                    }
                )
                .RequireAuthorization()
                .WithName("GetStockAmount")
                .WithOpenApi();
        }

        private static IResult UserAuthorization(string userId, HttpContext context)
        {
            if (userId == null)
                return Results.BadRequest();

            string? userIdFromToken = context.User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
                return Results.Unauthorized();

            if (userIdFromToken != userId)
                return Results.Forbid();
            return Results.Ok();
        }
    }
}
