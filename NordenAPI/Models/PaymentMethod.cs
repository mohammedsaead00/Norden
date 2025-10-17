using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NordenAPI.Models;

[Table("payment_methods")]
public class PaymentMethod
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("type")]
    public string Type { get; set; } = "card";

    [Required]
    [MaxLength(4)]
    [Column("card_last4")]
    public string CardLast4 { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("card_brand")]
    public string CardBrand { get; set; } = string.Empty; // Visa, Mastercard

    [Required]
    [Column("expiry_month")]
    public int ExpiryMonth { get; set; }

    [Required]
    [Column("expiry_year")]
    public int ExpiryYear { get; set; }

    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}

