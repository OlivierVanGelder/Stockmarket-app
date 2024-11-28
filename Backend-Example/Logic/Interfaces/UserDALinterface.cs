using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface UserDALinterface
    {
        string[] GetUsers();
        Task<bool> AddUserAsync(string name, string password);
        Task<bool> DeleteUser(string userId);
        Task<bool> VerifyUser(string name, string password);
        Task<string> GetUserId(string name);
        Task<double> GetUserBalance(string userId);
        Task<string> GetUserName(string userId);
        Task<bool> SellUserStock(string id, string ticker, int amount, double price);
        Task<bool> BuyUserStock(string id, string ticker, int amount, double price);
    }
}
