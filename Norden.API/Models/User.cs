using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Norden.API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        public string? AvatarUrl { get; set; }

        [MaxLength(120)]
        public string? GoogleId { get; set; }
        
        // These might still be needed for your internal app logic, keeping them
        public bool IsAdmin { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [MaxLength(255)]
        public string? RefreshToken { get; set; }
        
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
