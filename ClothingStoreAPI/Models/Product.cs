using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreAPI.Models;

[Table("Products")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("product_name")]
    public string ProductName { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Required]
    [Column("base_price", TypeName = "decimal(10, 2)")]
    public decimal BasePrice { get; set; }

    [MaxLength(100)]
    [Column("brand")]
    public string? Brand { get; set; }

    [MaxLength(255)]
    [Column("material")]
    public string? Material { get; set; }

    [Column("care_instructions")]
    public string? CareInstructions { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<Inventory> InventoryItems { get; set; } = new List<Inventory>();
    public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

