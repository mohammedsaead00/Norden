using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Norden.API.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrderId { get; set; }

        public Guid? ProductId { get; set; }

        [Required]
        [MaxLength(180)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [MaxLength(50)]
        public string? SelectedColor { get; set; }

        [MaxLength(10)]
        public string? SelectedSize { get; set; }

        public string? ImageUrl { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
