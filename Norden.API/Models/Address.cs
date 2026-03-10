using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Norden.API.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Label { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string City { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? State { get; set; }

        [Required]
        [MaxLength(80)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [Required]
        [MaxLength(30)]
        public string Phone { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
