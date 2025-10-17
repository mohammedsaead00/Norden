using System.ComponentModel.DataAnnotations;

namespace ClothingStoreAPI.DTOs;

public class PaymentDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}

public class CreatePaymentDto
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public string? TransactionId { get; set; }
}

public class UpdatePaymentDto
{
    public string? PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
}

