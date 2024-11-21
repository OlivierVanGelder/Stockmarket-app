using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace DAL.Tables
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = "";
        public int BalanceInCents { get; set; } = 0;
        public List<User_Stock> User_Stocks { get; set; } = new List<User_Stock>();
    }
}
