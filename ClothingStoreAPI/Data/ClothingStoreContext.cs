using Microsoft.EntityFrameworkCore;
using ClothingStoreAPI.Models;

namespace ClothingStoreAPI.Data;

public class ClothingStoreContext : DbContext
{
    public ClothingStoreContext(DbContextOptions<ClothingStoreContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Address
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Category (self-referencing)
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasOne(d => d.ParentCategory)
                .WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(d => d.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure ProductImage
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasOne(d => d.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Inventory
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.Size, e.Color }).IsUnique();
            
            entity.HasOne(d => d.Product)
                .WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Cart
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Inventory)
                .WithMany(p => p.CartItems)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.ShippingAddress)
                .WithMany(p => p.OrdersAsShipping)
                .HasForeignKey(d => d.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.BillingAddress)
                .WithMany(p => p.OrdersAsBilling)
                .HasForeignKey(d => d.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Inventory)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.OrderId).IsUnique();
            
            entity.HasOne(d => d.Order)
                .WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Review
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasOne(d => d.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

