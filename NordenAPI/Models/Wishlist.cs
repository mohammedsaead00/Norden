using System.ComponentModel.DataAnnotations;

namespace NordenAPI.Models
{
    public class Wishlist
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        
        public int ProductId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
