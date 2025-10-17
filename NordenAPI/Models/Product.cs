using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NordenAPI.Models;

public enum ProductCategory
{
    Coats,
    Blazers,
    DressShirts,
    Trousers,
    Accessories
}

[Table("products")]
public class Product
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required]
    [Column("category")]
    public ProductCategory Category { get; set; }

    [Column("images", TypeName = "json")]
    public string Images { get; set; } = "[]"; // JSON array of image URLs

    [Column("colors", TypeName = "json")]
    public string Colors { get; set; } = "[]"; // JSON array of color names

    [Column("sizes", TypeName = "json")]
    public string Sizes { get; set; } = "[]"; // JSON array of size names

    [Column("stock")]
    public int Stock { get; set; } = 0;

    [Column("is_new")]
    public bool IsNew { get; set; } = false;

    [Column("is_featured")]
    public bool IsFeatured { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
}

