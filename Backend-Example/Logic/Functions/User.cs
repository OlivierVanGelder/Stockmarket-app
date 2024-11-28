using Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Functions
{
    public class User
    {
        public async Task<string> GetUserName(UserDALinterface userDal, string userId)
        {
            return await userDal.GetUserName(userId);
        }
        public async Task<double> GetUserBalance(UserDALinterface userDal, string userId)
        {
            return await userDal.GetUserBalance(userId);
        }
        public async Task<bool> AddUser(UserDALinterface userDal, string name, string password)
        {
            return await userDal.AddUserAsync(name, password);
        }
        public async Task<bool> Deleteuser(UserDALinterface userDal, string userId)
        {
            return await userDal.DeleteUser(userId);
        }
        public async Task<bool> SellUserStock(UserDALinterface userDal, string id, string ticker, int amount, double price)
        {
            return await userDal.SellUserStock(id, ticker, amount, price);
        }
        public async Task<bool> BuyUserStock(UserDALinterface userDal, string id, string ticker, int amount, double price)
        {
            return await userDal.BuyUserStock(id, ticker, amount, price);
        }
    }
}
