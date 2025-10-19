using System.ComponentModel.DataAnnotations;

namespace NordenAPI.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid OrderId { get; set; }
        
        public int ProductId { get; set; }
        
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }
        
        public string? ImageUrl { get; set; }
    }
}
