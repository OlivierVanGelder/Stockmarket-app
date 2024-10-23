using System;
using System.Collections.Generic;
using System.Linq;
using Backend_Example.Data.BDaccess;
using Logic.Interfaces;

namespace DAL.BDaccess
{
    public class UserDAL : UserDALinterface
    {
        public string[] GetUsers()
        {
            using (var db = new DbContext())
            {
                var user = db.Users;
                return user.Select(s => s.Name).ToArray();
            }
        }
    }
}
