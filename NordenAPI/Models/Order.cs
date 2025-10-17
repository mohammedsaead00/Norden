using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NordenAPI.Models;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public enum PaymentMethodType
{
    Card,
    CashOnDelivery
}

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    [Column("order_number")]
    public string OrderNumber { get; set; } = string.Empty;

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    [Column("subtotal", TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }

    [Required]
    [Column("tax", TypeName = "decimal(10,2)")]
    public decimal Tax { get; set; } = 0;

    [Required]
    [Column("shipping", TypeName = "decimal(10,2)")]
    public decimal Shipping { get; set; } = 0;

    [Required]
    [Column("total", TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    [Required]
    [Column("payment_method")]
    public PaymentMethodType PaymentMethod { get; set; }

    [Column("shipping_address_id")]
    public Guid ShippingAddressId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("ShippingAddressId")]
    public virtual Address? ShippingAddress { get; set; }

    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

