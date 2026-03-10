using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Norden.API.Models
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

        public string? Images { get; set; } // JSON array of image URLs

        public bool IsVerified { get; set; } = false;

        public int HelpfulCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<ReviewHelpful> ReviewHelpfuls { get; set; } = new List<ReviewHelpful>();

        // Helper methods for JSON properties
        public List<string> GetImages()
        {
            return string.IsNullOrEmpty(Images) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(Images) ?? new List<string>();
        }

        public void SetImages(List<string> images)
        {
            Images = JsonSerializer.Serialize(images);
        }
    }

    public class ReviewHelpful
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ReviewId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ReviewId")]
        public virtual Review Review { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
