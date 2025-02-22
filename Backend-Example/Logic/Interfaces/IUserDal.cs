﻿using Logic.Models;

namespace Logic.Interfaces;
public interface IUserDal
{
    Task<StockAmount[]> GetUserStocks(string userId);
    Task<UserModel[]> GetAllUsers();
    Task<bool> AddUserAsync(string name, string password);
    Task<bool> DeleteUser(string userId);
    Task<bool> VerifyUser(string name, string password);
    Task<bool> VerifyNewUser(string name);
    Task<string> GetUserId(string name);
    Task<bool> IsAdmin(string userId);
    Task<double> GetUserBalance(string userId);
    Task<string> GetUserName(string userId);
    Task<bool> SellUserStock(string id, string ticker, int amount, double price);
    Task<bool> BuyUserStock(string id, string ticker, int amount, double price);
    Task<double> GetUserStockAmount(string id, string ticker);
    Task<bool> IsFrozen(string userId);
    Task<bool> Freeze(string userId);
    Task<bool> UnFreeze(string userId);
}
