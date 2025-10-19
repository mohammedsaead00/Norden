using System.ComponentModel.DataAnnotations;

namespace NordenAPI.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string OrderNumber { get; set; } = string.Empty;
        
        public Guid UserId { get; set; }
        
        [Required]
        public string Status { get; set; } = "Pending";
        
        public decimal Total { get; set; }
        
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
