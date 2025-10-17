using System.ComponentModel.DataAnnotations;

namespace ClothingStoreAPI.DTOs;

public class CartDto
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int InventoryId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime AddedAt { get; set; }
}

public class AddToCartDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int InventoryId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}

public class UpdateCartDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

