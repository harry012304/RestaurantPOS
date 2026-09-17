using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Data;
using RestaurantPOS.Models;

namespace RestaurantPOS.Services
{
    public class AuthService
    {
        public async Task<User?> LoginAsync(
            string username,
            string password)
        {
            using var context = new AppDbContext();

            var user = await context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username &&
                    u.IsActive);

            if (user == null)
            {
                return null;
            }

            bool passwordCorrect =
                BCrypt.Net.BCrypt.Verify(
                    password,
                    user.PasswordHash);

            if (!passwordCorrect)
            {
                return null;
            }

            return user;
        }
    }
}