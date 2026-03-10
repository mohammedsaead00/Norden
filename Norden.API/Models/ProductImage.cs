using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Norden.API.Models
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 0;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
