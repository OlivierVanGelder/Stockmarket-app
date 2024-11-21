using Backend_Example.Data.BDaccess;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tables
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public int BalanceInCents { get; set; } = 0;
        public List<User_Stock> User_Stocks { get; set; } = [];
    }
}
