using DAL.Tables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.DbAccess
{
    public class DbStockEngine : IdentityDbContext<User>
    {
        public DbStockEngine() { }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<CandlestockMinute> Candles { get; set; }
        public DbSet<User_Stock> User_Stocks { get; set; }

        public DbStockEngine(DbContextOptions<DbStockEngine> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conn = Environment.GetEnvironmentVariable("ConnectionString");
                optionsBuilder.UseSqlServer(
                    conn ?? @"Server=(localdb)\LOCAL;Database=StockEngine;Integrated Security=True;"
                );
            }
        }
    }
}
