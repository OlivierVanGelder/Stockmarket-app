using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DAL.Tables
{
    public class User : IdentityUser
    {
        public override string UserName { get; set; } = "";
        public override string Email { get; set; } = "";
        public override string NormalizedUserName { get; set; } = "";
        public override string NormalizedEmail { get; set; } = "";
        public override string PasswordHash { get; set; } = "";
        public override string SecurityStamp { get; set; } = "";

        public int BalanceInCents { get; set; } = 0;
        public List<User_Stock> User_Stocks { get; set; } = new List<User_Stock>();
    }
}
