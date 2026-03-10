using Microsoft.EntityFrameworkCore;
using Norden.API.Models;

namespace Norden.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewHelpful> ReviewHelpfuls { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderTracking> OrderTrackings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.GoogleId).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Phone).HasMaxLength(30);

                entity.HasOne(u => u.Cart)
                    .WithOne(c => c.User)
                    .HasForeignKey<Cart>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // Season configuration
            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(180);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Season)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.SeasonId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ProductImage configuration
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(pi => pi.Product)
                    .WithMany(p => p.Images)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductVariant configuration
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ProductId, e.Color, e.Size }).IsUnique();
                entity.HasOne(pv => pv.Product)
                    .WithMany(p => p.Variants)
                    .HasForeignKey(pv => pv.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Review configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Comment).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(200);
                
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ReviewHelpful configuration
            modelBuilder.Entity<ReviewHelpful>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ReviewId, e.UserId }).IsUnique();

                entity.HasOne(e => e.Review)
                    .WithMany(r => r.ReviewHelpfuls)
                    .HasForeignKey(e => e.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Cart configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();
            });

            // CartItem configuration
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.CartId, e.ProductId, e.SelectedColor, e.SelectedSize }).IsUnique();

                entity.HasOne(e => e.Cart)
                    .WithMany(u => u.CartItems)
                    .HasForeignKey(e => e.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // WishlistItem configuration
            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.WishlistItems)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.WishlistItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Address configuration
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Label).IsRequired().HasMaxLength(40);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Street).IsRequired().HasMaxLength(255);
                entity.Property(e => e.City).IsRequired().HasMaxLength(80);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(80);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(30);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(20);
                
                entity.Property(e => e.SubTotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Shipping).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Tax).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(180);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // OrderTracking configuration
            modelBuilder.Entity<OrderTracking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Location).HasMaxLength(200);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderTrackings)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
