using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreAPI.Models;

[Table("Payments")]
public class Payment
{
    [Key]
    [Column("payment_id")]
    public int PaymentId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty; // 'credit_card', 'debit_card', 'paypal', 'stripe', 'cash_on_delivery'

    [Required]
    [MaxLength(20)]
    [Column("payment_status")]
    public string PaymentStatus { get; set; } = "pending"; // 'pending', 'completed', 'failed', 'refunded'

    [MaxLength(255)]
    [Column("transaction_id")]
    public string? TransactionId { get; set; }

    [Required]
    [Column("amount", TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column("payment_date")]
    public DateTime PaymentDate { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }
}

