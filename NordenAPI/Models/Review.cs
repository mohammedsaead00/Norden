using System.ComponentModel.DataAnnotations;

namespace NordenAPI.Models
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [Required]
        public string Comment { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
