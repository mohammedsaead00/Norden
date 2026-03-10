using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Norden.API.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Season { get; set; } = "all"; // winter, summer, all

        [MaxLength(60)]
        public string? IconName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
