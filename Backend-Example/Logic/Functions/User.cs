using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Interfaces;

namespace Logic.Functions
{
    public class User
    {
        public async Task<string> GetUserName(IUserDAL userDal, string userId)
        {
            return await userDal.GetUserName(userId);
        }

        public async Task<double> GetUserBalance(IUserDAL userDal, string userId)
        {
            return await userDal.GetUserBalance(userId);
        }

        public async Task<bool> AddUser(IUserDAL userDal, string name, string password)
        {
            if (await userDal.VerifyNewUser(name))
            {
                return false;
            }
            return await userDal.AddUserAsync(name, password);
        }

        public async Task<bool> Deleteuser(IUserDAL userDal, string userId)
        {
            return await userDal.DeleteUser(userId);
        }

        public async Task<bool> SellUserStock(
            IUserDAL userDal,
            string id,
            string ticker,
            int amount,
            double price
        )
        {
            return await userDal.SellUserStock(id, ticker, amount, price);
        }

        public async Task<bool> BuyUserStock(
            IUserDAL userDal,
            string id,
            string ticker,
            int amount,
            double price
        )
        {
            return await userDal.BuyUserStock(id, ticker, amount, price);
        }

        /// <exception cref="ArgumentException"></exception>
        public async Task<double> GetUserStockAmount(IUserDAL userDal, string id, string ticker)
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
}
