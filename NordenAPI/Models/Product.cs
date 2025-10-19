using System.ComponentModel.DataAnnotations;

namespace NordenAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public decimal Price { get; set; }
        
        public int CategoryId { get; set; }
        
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        
        public int Stock { get; set; } = 0;
        
        public bool IsNew { get; set; } = false;
        
        public bool IsFeatured { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
