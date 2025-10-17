using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreAPI.Models;

[Table("Inventory")]
public class Inventory
{
    [Key]
    [Column("inventory_id")]
    public int InventoryId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("size")]
    public string Size { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("color")]
    public string Color { get; set; } = string.Empty;

    [Column("quantity_in_stock")]
    public int QuantityInStock { get; set; } = 0;

    [Column("reorder_level")]
    public int ReorderLevel { get; set; } = 10;

    [Column("last_restocked")]
    public DateTime? LastRestocked { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }

    public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

