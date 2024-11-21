using System;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;
using Backend_Example.Data.BDaccess;
using Logic.Interfaces;
using DAL.Tables;
using Microsoft.EntityFrameworkCore;

namespace DAL.BDaccess
{
    public class UserDAL : UserDALinterface
    {
        private readonly Backend_Example.Data.BDaccess.DbContext _context; // Inject ApplicationDbContext

        // Constructor injection of ApplicationDbContext
        public UserDAL(Backend_Example.Data.BDaccess.DbContext context)
        {
            _context = context;
        }

        public string[] GetUsers()
        {
            var users = _context.Users;
            return users.Select(u => u.Name).ToArray();
        }

        public void WriteUser(string name, string password)
        {
            var user = new User
            {
                Name = name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), // Store hashed password
                BalanceInCents = 0,
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public bool VerifyUser(string name, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == name);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public bool ChangeUserStock(string name, string ticker, int amount)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == name);
            var stock = _context.Stocks.FirstOrDefault(s => s.Ticker == ticker);
            if (user == null || stock == null)
            {
                return false;
            }

            var userStock = _context.User_Stocks.FirstOrDefault(us => us.UserId == user.Id && us.StockId == stock.Id);
            if (userStock == null)
            {
                var newUserStock = new User_Stock
                {
                    UserId = user.Id,  // user.Id is now a string
                    StockId = stock.Id,
                    StockAmount = amount,
                };
                _context.User_Stocks.Add(newUserStock);
            }
            else
            {
                userStock.StockAmount += amount;
            }
            _context.SaveChanges();
            return true;
        }
    }
}