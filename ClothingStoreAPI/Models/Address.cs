using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreAPI.Models;

[Table("Addresses")]
public class Address
{
    [Key]
    [Column("address_id")]
    public int AddressId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("address_type")]
    public string AddressType { get; set; } = string.Empty; // 'shipping', 'billing', 'both'

    [Required]
    [MaxLength(255)]
    [Column("street_address")]
    public string StreetAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("apartment")]
    public string? Apartment { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("city")]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("state")]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("postal_code")]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("country")]
    public string Country { get; set; } = "USA";

    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [InverseProperty("ShippingAddress")]
    public virtual ICollection<Order> OrdersAsShipping { get; set; } = new List<Order>();

    [InverseProperty("BillingAddress")]
    public virtual ICollection<Order> OrdersAsBilling { get; set; } = new List<Order>();
}

