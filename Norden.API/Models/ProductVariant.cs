using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Norden.API.Models
{
    public class ProductVariant
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Size { get; set; } = string.Empty;

        public int Stock { get; set; } = 0;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
