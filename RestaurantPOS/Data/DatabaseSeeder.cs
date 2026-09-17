using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Models;

namespace RestaurantPOS.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedUsersAsync()
        {
            using var context = new AppDbContext();

            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                context.Users.Add(new User
                {
                    FullName = "System Administrator",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    IsActive = true
                });
            }

            if (!await context.Users.AnyAsync(u => u.Username == "cashier"))
            {
                context.Users.Add(new User
                {
                    FullName = "Main Cashier",
                    Username = "cashier",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("cashier123"),
                    Role = "Cashier",
                    IsActive = true
                });
            }

            await context.SaveChangesAsync();
        }
    }
}