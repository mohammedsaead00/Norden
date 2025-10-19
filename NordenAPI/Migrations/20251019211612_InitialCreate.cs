using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NordenAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    ShippingAddress = table.Column<string>(type: "text", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wishlist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlist", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "ImageUrl", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(4720), "Winter coats and jackets", "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=200", "Coats", null },
                    { 2, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(4726), "Professional blazers", "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200", "Blazers", null },
                    { 3, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(4728), "Dress shirts and casual shirts", "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=200", "Shirts", null },
                    { 4, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(4729), "Designer jeans", "https://images.unsplash.com/photo-1542272604-787c3835535d?w=200", "Jeans", null },
                    { 5, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(4730), "Luxury sweaters", "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=200", "Sweaters", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsFeatured", "IsNew", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(5154), "Premium wool coat for winter", "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400", true, true, "Luxury Wool Coat", 299.99m, 50, null },
                    { 2, 2, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(5169), "Professional blazer for business", "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400", true, false, "Classic Blazer", 199.99m, 30, null },
                    { 3, 3, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(5172), "Premium silk dress shirt", "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=400", false, true, "Silk Dress Shirt", 89.99m, 75, null },
                    { 4, 4, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(5174), "High-quality designer jeans", "https://images.unsplash.com/photo-1542272604-787c3835535d?w=400", true, false, "Designer Jeans", 149.99m, 40, null },
                    { 5, 5, new DateTime(2025, 10, 19, 21, 16, 12, 83, DateTimeKind.Utc).AddTicks(5176), "Luxury cashmere sweater", "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=400", false, true, "Cashmere Sweater", 179.99m, 25, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Wishlist");
        }
    }
}
