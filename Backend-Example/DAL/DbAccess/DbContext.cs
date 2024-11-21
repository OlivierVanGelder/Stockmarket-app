using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DAL.Tables;

namespace Backend_Example.Data.BDaccess
{
    public class DbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<CandlestockMinute> Candles { get; set; }
        public DbSet<User_Stock> User_Stocks { get; set; }

        public DbContext(DbContextOptions<DbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conn = Environment.GetEnvironmentVariable("ConnectionString");
                optionsBuilder.UseSqlServer(
                    conn ?? @"Server=(localdb)\LOCAL;Database=StockEngine;Integrated Security=True;");
            }
        }
    }
}
