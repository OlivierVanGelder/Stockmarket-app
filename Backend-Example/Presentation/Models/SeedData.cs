using System.Linq;
using Backend_Example.Data.BDaccess;
using DAL.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<DbStockEngine>();

        // Apply any pending migrations (this will ensure the schema is up to date)
        context.Database.Migrate();

        // Add other tables as necessary
        if (!context.Users.Any()) // Check if users table is empty
        {
            var user = new User
            {
                UserName = "test",
                PasswordHash =
                    "AQAAAAIAAYagAAAAELv5kpFumetRehfEMc/SxPNnALzqWXiV/CrNtK6nYpgVBI7AaCrRyKYrUkxbienI4g==",
            };

            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
