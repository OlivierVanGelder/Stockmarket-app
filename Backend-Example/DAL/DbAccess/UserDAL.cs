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

        public void WriteUser(string name, string password)
        {
            using (var db = new DbContext())
            {
                var user = new User
                {
                    Name = name,
                    Password = password,
                    BalanceInCents = 0,
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public bool VerifyUser(string name, string password)
        {
            using (var db = new DbContext())
            {
                var user = db.Users;
                return user.Any(u => u.Name == name && u.Password == password);
            }
        }

        public bool ChangeUserStock(string name, string ticker, int amount)
        {
            using var db = new DbContext();

            var user = db.Users.Where(u => u.Name == name).FirstOrDefault();
            var stock = db.Stocks.Where(s => s.Ticker == ticker).FirstOrDefault();
            if (user == null || stock == null)
            {
                return false;
            }

            var userStock = db
                .User_Stocks.Where(us => us.UserId == user.Id && us.StockId == stock.Id)
                .FirstOrDefault();
            if (userStock == null)
            {
                var newUserStock = new User_Stock
                {
                    UserId = user.Id,
                    StockId = stock.Id,
                    StockAmount = amount,
                };
                db.User_Stocks.Add(newUserStock);
            }
            else
            {
                userStock.StockAmount += amount;
            }
            db.SaveChanges();
            return true;
        }
    }
}
