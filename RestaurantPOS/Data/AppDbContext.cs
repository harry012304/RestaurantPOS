using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Models;

namespace RestaurantPOS.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<RestaurantTable> RestaurantTables { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=restaurantpos.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RestaurantTable>().HasData(
                new RestaurantTable
                {
                    Id = 1,
                    TableNumber = "T01",
                    Capacity = 2,
                    Status = "Available"
                },
                new RestaurantTable
                {
                    Id = 2,
                    TableNumber = "T02",
                    Capacity = 2,
                    Status = "Available"
                },
                new RestaurantTable
                {
                    Id = 3,
                    TableNumber = "T03",
                    Capacity = 4,
                    Status = "Available"
                },
                new RestaurantTable
                {
                    Id = 4,
                    TableNumber = "T04",
                    Capacity = 4,
                    Status = "Available"
                },
                new RestaurantTable
                {
                    Id = 5,
                    TableNumber = "T05",
                    Capacity = 4,
                    Status = "Available"
                },
                new RestaurantTable
                {
                    Id = 6,
                    TableNumber = "T06",
                    Capacity = 6,
                    Status = "Available"
                },
                new RestaurantTable
                {
                    Id = 7,
                    TableNumber = "T07",
                    Capacity = 6,
                    Status = "Available"
                },
                new RestaurantTable
                {
                    Id = 8,
                    TableNumber = "T08",
                    Capacity = 8,
                    Status = "Available"
                }
            );

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Main Course"
                },
                new Category
                {
                    Id = 2,
                    Name = "Drinks"
                },
                new Category
                {
                    Id = 3,
                    Name = "Desserts"
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Chicken Chop",
                    Price = 15.90m,
                    StockQuantity = 30,
                    IsAvailable = true,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Carbonara",
                    Price = 18.00m,
                    StockQuantity = 25,
                    IsAvailable = true,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Beef Burger",
                    Price = 12.90m,
                    StockQuantity = 35,
                    IsAvailable = true,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 4,
                    Name = "Iced Coffee",
                    Price = 6.50m,
                    StockQuantity = 50,
                    IsAvailable = true,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 5,
                    Name = "Lemon Tea",
                    Price = 5.50m,
                    StockQuantity = 50,
                    IsAvailable = true,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 6,
                    Name = "Chocolate Cake",
                    Price = 8.90m,
                    StockQuantity = 15,
                    IsAvailable = true,
                    CategoryId = 3
                }
            );
        }
    }
}