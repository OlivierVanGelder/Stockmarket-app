using System.Diagnostics;
using DAL.Tables;
using Logic.Interfaces;
using Logic.Models;
using Microsoft.AspNetCore.Identity;

namespace DAL.DbAccess;

public class UserDal : IUserDal
{
    private const int InitialBalanceCents = 500000;
    private readonly DbStockEngine _context;
    private readonly UserManager<User> _userManager;

    public UserDal(DbStockEngine context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<UserModel[]> GetAllUsers()
    {
        var users = _context.Users.ToList();
        List<UserModel> userModels = new List<UserModel>();

        foreach (var user in users)
        {
            // Fetch the user balance
            double balance = await GetUserBalance(user.Id);

            // Fetch the total stock value for the user
            double totalStockValue = 0;
            var userStocks = await GetUserStocks(user.Id);

            // Sum up the value of each stock the user holds
            foreach (var stockAmount in userStocks)
            {
                totalStockValue += stockAmount.TotalValue;
            }

            // Calculate total balance (balance in cents + stock value)
            double totalBalance = balance + totalStockValue;

            // Create a new UserModel and add to the list
            var userModel = new UserModel(user.Id, user.UserName, (int)(totalBalance * 100)); // Store balance in cents
            userModels.Add(userModel);
        }

        return userModels.ToArray();
    }


    public async Task<bool> AddUserAsync(string name, string password)
    {
        var user = new User { UserName = name, BalanceInCents = InitialBalanceCents };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return true;
        }
        foreach (var error in result.Errors)
        {
            Debug.WriteLine(error.Description);
        }

        return false;
    }

    public async Task<bool> VerifyNewUser(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user != null;
    }

    public async Task<bool> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return true;
        }
        foreach (var error in result.Errors)
        {
            Debug.WriteLine(error.Description);
        }

        return false;
    }

    private async Task<bool> UpdateUserBalance(string id, double balance)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.BalanceInCents = (int)(balance * 100);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return true;
        }
        foreach (var error in result.Errors)
        {
            Debug.WriteLine(error.Description);
        }

        return false;
    }

    public async Task<bool> UpdateUserBalance(string id, double balance, double change)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.BalanceInCents = (int)(balance * 100);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return true;
        }
        foreach (var error in result.Errors)
        {
            Debug.WriteLine(error.Description);
        }

        return false;
    }

    /// <exception cref="ArgumentException"></exception>
    public async Task<double> GetUserStockAmount(string userId, string ticker)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var stock = _context.Stocks.FirstOrDefault(s => s.Ticker == ticker);
        if (user == null || stock == null)
        {
            throw new ArgumentException("User or stock not found");
        }
        var userStock = _context.User_Stocks.FirstOrDefault(us =>
            us.UserId == user.Id && us.StockId == stock.Id
        );
        if (userStock == null)
        {
            return 0;
        }
        return userStock.StockAmount;
    }

    public async Task<StockAmount[]> GetUserStocks(string userId)
    {
        var allStocks = _context.Stocks.ToList();
        List<StockAmount> allUserStockAmounts = new List<StockAmount>();

        foreach (var stock in allStocks)
        {
            var stockDal = new StockDal(_context);
            var userStock = _context.User_Stocks.FirstOrDefault(us => us.UserId == userId && us.StockId == stock.Id);
            var price = await stockDal.GetStockPrice(stock.Ticker);
            var stockAmountValue = userStock != null ? userStock.StockAmount : 0;
            StockAmount stockAmount = new StockAmount(stock.Ticker, stockAmountValue, price);
            allUserStockAmounts.Add(stockAmount);
        }

        return allUserStockAmounts.ToArray();
    }


    public async Task<bool> BuyUserStock(string id, string ticker, int amount, double price)
    {
        var user = await _userManager.FindByIdAsync(id);
        var stock = _context.Stocks.FirstOrDefault(s => s.Ticker == ticker);
        if (user == null || stock == null)
        {
            return false;
        }

        var userStock = _context.User_Stocks.FirstOrDefault(us =>
            us.UserId == user.Id && us.StockId == stock.Id
        );
        if (userStock == null)
        {
            var newUserStock = new User_Stock
            {
                UserId = user.Id,
                StockId = stock.Id,
                StockAmount = amount
            };
            _context.User_Stocks.Add(newUserStock);
        }
        else
        {
            userStock.StockAmount += amount;
        }
        var oldBalance = await GetUserBalance(id);
        var newBalance = oldBalance - (price * amount);
        if (!await UpdateUserBalance(id, newBalance))
        {
            return false;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SellUserStock(string id, string ticker, int amount, double price)
    {
        var user = await _userManager.FindByIdAsync(id);
        var stock = _context.Stocks.FirstOrDefault(s => s.Ticker == ticker);
        if (user == null || stock == null)
        {
            return false;
        }

        var userStock = _context.User_Stocks.FirstOrDefault(us =>
            us.UserId == user.Id && us.StockId == stock.Id
        );
        if (userStock == null)
        {
            var newUserStock = new User_Stock
            {
                UserId = user.Id,
                StockId = stock.Id,
                StockAmount = amount
            };
            _context.User_Stocks.Add(newUserStock);
        }
        else
        {
            if (userStock.StockAmount - amount < 0)
            {
                return false;
            }
            userStock.StockAmount -= amount;
        }
        var oldBalance = await GetUserBalance(id);
        var newBalance = oldBalance + price * amount;
        await UpdateUserBalance(id, newBalance);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerifyUser(string name, string password)
    {
        var user = await _userManager.FindByNameAsync(name);
        if (user == null)
            return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<string> GetUserId(string name)
    {
        var userId = (await _userManager.FindByNameAsync(name) ?? new User()).Id;
        return userId;
    }

    public async Task<bool> IsAdmin(string id)
    {
        var isAdmin = (await _userManager.FindByIdAsync(id) ?? new User()).IsAdmin;
        return isAdmin;
    }

    public async Task<double> GetUserBalance(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var userBalance = (user ?? new User()).BalanceInCents / 100.0;

        return userBalance;
    }

    public async Task<string> GetUserName(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var userName = (user ?? new User()).UserName;

        return userName;
    }
}