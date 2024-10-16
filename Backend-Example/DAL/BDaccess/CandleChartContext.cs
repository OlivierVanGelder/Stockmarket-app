using System;
using System.Collections.Generic;
using Backend_Example.Data.Models;
using Backend_Example.Logic.Classes;
using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Data.BDaccess
{
    public class CandleChartContext : DbContext
    {
        public DbSet<CandleStick> Candlesticks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=StockEngine;Integrated Security=True;"
            );
    }
}
