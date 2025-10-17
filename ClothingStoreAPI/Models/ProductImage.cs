using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreAPI.Models;

[Table("Product_Images")]
public class ProductImage
{
    [Key]
    [Column("image_id")]
    public int ImageId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("is_primary")]
    public bool IsPrimary { get; set; } = false;

    [Column("display_order")]
    public int DisplayOrder { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}

