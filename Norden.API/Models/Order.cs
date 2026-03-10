using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Norden.API.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        public Guid? AddressId { get; set; }

        [Required]
        [MaxLength(30)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // Pending, Confirmed, Shipped, Delivered, Cancelled

        [Required]
        [MaxLength(20)]
        public string PaymentMethod { get; set; } = string.Empty; // CreditCard, CashOnDelivery

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Shipping { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Tax { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [MaxLength(30)]
        public string? PromoCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("AddressId")]
        public virtual Address? Address { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<OrderTracking> OrderTrackings { get; set; } = new List<OrderTracking>();
    }
}
