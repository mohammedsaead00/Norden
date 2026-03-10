using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Norden.API.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CartId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [MaxLength(50)]
        public string? SelectedColor { get; set; }

        [MaxLength(10)]
        public string? SelectedSize { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
