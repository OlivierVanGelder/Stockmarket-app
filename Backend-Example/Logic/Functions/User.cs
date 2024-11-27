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
            return await userDal.GetUserName(userId) ;
        }

        public async Task<double> GetUserBalance(UserDALinterface userDal, string userId)
        {
            return await userDal.GetUserBalance(userId);
        }
    }
}
