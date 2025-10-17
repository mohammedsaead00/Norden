using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreAPI.Models;

[Table("Orders")]
public class Order
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("order_date")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Required]
    [Column("total_amount", TypeName = "decimal(10, 2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("order_status")]
    public string OrderStatus { get; set; } = "pending"; // 'pending', 'processing', 'shipped', 'delivered', 'cancelled'

    [Column("shipping_address_id")]
    public int ShippingAddressId { get; set; }

    [Column("billing_address_id")]
    public int BillingAddressId { get; set; }

    [MaxLength(100)]
    [Column("tracking_number")]
    public string? TrackingNumber { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("ShippingAddressId")]
    public virtual Address? ShippingAddress { get; set; }

    [ForeignKey("BillingAddressId")]
    public virtual Address? BillingAddress { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual Payment? Payment { get; set; }
}

