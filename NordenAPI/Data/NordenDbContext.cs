using Microsoft.EntityFrameworkCore;
using NordenAPI.Models;

namespace NordenAPI.Data
{
    public class NordenDbContext : DbContext
    {
        public NordenDbContext(DbContextOptions<NordenDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Coats", Description = "Winter coats and jackets", ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=200" },
                new Category { Id = 2, Name = "Blazers", Description = "Professional blazers", ImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200" },
                new Category { Id = 3, Name = "Shirts", Description = "Dress shirts and casual shirts", ImageUrl = "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=200" },
                new Category { Id = 4, Name = "Jeans", Description = "Designer jeans", ImageUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=200" },
                new Category { Id = 5, Name = "Sweaters", Description = "Luxury sweaters", ImageUrl = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=200" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Luxury Wool Coat", Description = "Premium wool coat for winter", Price = 299.99m, CategoryId = 1, ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400", Stock = 50, IsNew = true, IsFeatured = true },
                new Product { Id = 2, Name = "Classic Blazer", Description = "Professional blazer for business", Price = 199.99m, CategoryId = 2, ImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400", Stock = 30, IsNew = false, IsFeatured = true },
                new Product { Id = 3, Name = "Silk Dress Shirt", Description = "Premium silk dress shirt", Price = 89.99m, CategoryId = 3, ImageUrl = "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=400", Stock = 75, IsNew = true, IsFeatured = false },
                new Product { Id = 4, Name = "Designer Jeans", Description = "High-quality designer jeans", Price = 149.99m, CategoryId = 4, ImageUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=400", Stock = 40, IsNew = false, IsFeatured = true },
                new Product { Id = 5, Name = "Cashmere Sweater", Description = "Luxury cashmere sweater", Price = 179.99m, CategoryId = 5, ImageUrl = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=400", Stock = 25, IsNew = true, IsFeatured = false }
            );
        }
    }
}
