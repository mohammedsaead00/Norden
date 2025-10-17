using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NordenAPI.Models;

[Table("cart_items")]
public class CartItem
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("cart_id")]
    public Guid CartId { get; set; }

    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Required]
    [Column("quantity")]
    public int Quantity { get; set; } = 1;

    [Required]
    [MaxLength(50)]
    [Column("selected_color")]
    public string SelectedColor { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    [Column("selected_size")]
    public string SelectedSize { get; set; } = string.Empty;

    [Required]
    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Column("added_at")]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("CartId")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}

