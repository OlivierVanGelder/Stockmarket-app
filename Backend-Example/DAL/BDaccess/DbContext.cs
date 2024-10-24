using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Data.BDaccess
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<CandleStockMinute> Candles { get; set; }
        public DbSet<User_Stock> User_Stocks { get; set; }

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
        public int BalanceInCents { get; set; }
        public List<User_Stock> User_Stocks { get; set; }
    }

    [Table("User_Stock")]
    public class User_Stock
    {
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

    [Table("CandleStockMinute")]
    public class CandleStockMinute
    {
        public int Id { get; set; }
        public int Stock_Id { get; set; }
        public DateTime Date { get; set; }
        public int Open { get; set; }
        public int High { get; set; }
        public int Low { get; set; }
        public int Close { get; set; }
        public int Volume { get; set; }
    }
}
