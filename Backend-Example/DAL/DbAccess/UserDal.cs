using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend_Example.Data.BDaccess;
using DAL.Tables;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DAL.BDaccess
{
    public class UserDAL : IUserDAL
    {
        private const int INITIALBALANCECENTS = 500000;
        private readonly DbStockEngine _context;
        private readonly UserManager<User> _userManager;

        public UserDAL(DbStockEngine context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string[] GetUsers()
        {
            var users = _context.Users;
            return users.Select(u => u.UserName).ToArray();
        }

        public async Task<bool> AddUserAsync(string name, string password)
        {
            var user = new User { UserName = name, BalanceInCents = INITIALBALANCECENTS };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine(error.Description);
                }

                return false;
            }
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
            else
            {
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine(error.Description);
                }

                return false;
            }
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
            else
            {
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine(error.Description);
                }

                return false;
            }
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
            else
            {
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine(error.Description);
                }

                return false;
            }
        }

        /// <exception cref="ArgumentException"></exception>
        public async Task<double> GetUserStockAmount(string id, string ticker)
        {
            var user = await _userManager.FindByIdAsync(id);
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
                    StockAmount = amount,
                };
                _context.User_Stocks.Add(newUserStock);
            }
            else
            {
                userStock.StockAmount += amount;
            }
            double oldBalance = await GetUserBalance(id);
            double newBalance = oldBalance - (price * amount);
            await UpdateUserBalance(id, newBalance);

            _context.SaveChanges();
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
                    StockAmount = amount,
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
            double oldBalance = await GetUserBalance(id);
            double newBalance = oldBalance + (price * amount);
            await UpdateUserBalance(id, newBalance);

            _context.SaveChanges();
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
            var userId = (await _userManager.FindByNameAsync(name)).Id;
            return userId;
        }

        public async Task<double> GetUserBalance(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            double userBalance = user.BalanceInCents / 100.0;

            return userBalance;
        }

        public async Task<string> GetUserName(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            string userName = user.UserName;

            return userName;
        }
    }
}
