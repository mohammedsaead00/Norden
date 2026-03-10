using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Norden.API.Models
{
    public class Season
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? BannerImageUrl { get; set; }

        [MaxLength(10)]
        public string AccentColor { get; set; } = "#D4AF37";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
