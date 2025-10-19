using System.ComponentModel.DataAnnotations;

namespace NordenAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
