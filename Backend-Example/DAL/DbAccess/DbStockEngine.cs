using DAL.Tables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.DbAccess;

public class DbStockEngine : IdentityDbContext<User>
{
    public DbStockEngine() { }

    public DbSet<Stock> Stocks { get; set; } = null!;
    public DbSet<CandlestockMinute> Candles { get; set; } = null!;
    public DbSet<User_Stock> User_Stocks { get; set; } = null!;

    public DbStockEngine(DbContextOptions<DbStockEngine> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var conn = Environment.GetEnvironmentVariable("DefaultConnection");
        optionsBuilder.UseSqlServer(
            conn ?? @"Server=(localdb)\\TestLocalServer;Database=TestDatabase;Integrated Security=True;"
        );
    }
}
