using System.ComponentModel.DataAnnotations.Schema;
using DAL.Tables;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Data.BDaccess
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<CandlestockMinute> Candles { get; set; }
        public DbSet<User_Stock> User_Stocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var conn = Environment.GetEnvironmentVariable("ConnectionString");
            if (conn != null)
                Console.WriteLine($"Using connection string: {conn}");
            optionsBuilder.UseSqlServer(
                conn ?? @"Server=(localdb)\LOCAL;Database=StockEngine;Integrated Security=True;"
            );
        }
    }
}
