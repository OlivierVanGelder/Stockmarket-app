﻿using Logic.Interfaces;
using Logic.Models;

namespace Logic.Functions;
public static class User
{
    public static async Task<StockAmount[]> GetUserStocks(IUserDal userDal, string userId)
    {
        return await userDal.GetUserStocks(userId);
    }
    
    public static async Task<string> GetUserName(IUserDal userDal, string userId)
    {
        return await userDal.GetUserName(userId);
    }

    public static async Task<double> GetUserBalance(IUserDal userDal, string userId)
    {
        return await userDal.GetUserBalance(userId);
    }

    public static async Task<bool> IsAdmin(IUserDal userDal, string userId)
    {
        return await userDal.IsAdmin(userId);
    }

    public static async Task<bool> IsFrozen(IUserDal userDal, string userId)
    {
        return await userDal.IsFrozen(userId);
    }
    
    public static async Task<bool> FreezeUser(IUserDal userDal, string userId)
    {
        return await userDal.Freeze(userId);
    }    public static async Task<bool> UnFreezeUser(IUserDal userDal, string userId)
    {
        return await userDal.UnFreeze(userId);
    }
    
    public static async Task<bool> VerifyUser(IUserDal userDal, string name, string password)
    {
        return await userDal.VerifyUser(name,password);
    }

    public static async Task<bool> AddUser(IUserDal userDal, string name, string password)
    {
        if (await userDal.VerifyNewUser(name))
        {
            return false;
        }
        return await userDal.AddUserAsync(name, password);
    }

    public static async Task<string> GetUserId(IUserDal userDal, string name)
    {
        return await userDal.GetUserId(name);
    }

    public static async Task<bool> DeleteUser(IUserDal userDal, string userId)
    {
        return await userDal.DeleteUser(userId);
    }

    public static async Task<UserModel[]> GetAllUsers(IUserDal userDal)
    {
        return await userDal.GetAllUsers();
    }
    
    public static async Task<bool> SellUserStock(
        IUserDal userDal,
        string id,
        string ticker,
        int amount,
        double price
    )
    {
        return await userDal.SellUserStock(id, ticker, amount, price);
    }

    public static async Task<bool> BuyUserStock(
        IUserDal userDal,
        string id,
        string ticker,
        int amount,
        double price
    )
    {
        return await userDal.BuyUserStock(id, ticker, amount, price);
    }

    /// <exception cref="ArgumentException"></exception>
    public static async Task<double> GetUserStockAmount(IUserDal userDal, string id, string ticker)
    {
        try
        {
            return await userDal.GetUserStockAmount(id, ticker);
        }
        catch (ArgumentException e)
        {
            throw new ArgumentException(e.Message);
        }
    }
}
