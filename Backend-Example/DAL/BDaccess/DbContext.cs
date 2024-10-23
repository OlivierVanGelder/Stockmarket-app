using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Data.BDaccess
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlServer(
                @"Server=(localdb)\LOCAL;Database=StockEngine;Integrated Security=True;"
            );
    }

    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string BalanceInCents { get; set; }
        public List<User_Stock> User_Stocks { get; set; }
    }

    public class User_Stock
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StockId { get; set; }
        public int StockAmount { get; set; }
    }

    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public int PriceInCents { get; set; }
    }
}
