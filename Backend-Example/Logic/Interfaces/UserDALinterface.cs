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
        Task<bool> VerifyUser(string name, string password);
        Task<string> GetUserId(string name);
        Task<double> GetUserBalance(string userId);
        Task<string> GetUserName(string userId);
        public bool ChangeUserStock(string userId, string stockId, int amount);
    }
}
