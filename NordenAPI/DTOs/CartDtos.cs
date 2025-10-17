using System.ComponentModel.DataAnnotations;

namespace NordenAPI.DTOs;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }
}

public class CartItemDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string SelectedColor { get; set; } = string.Empty;
    public string SelectedSize { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}

public class AddToCartRequest
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    [Required]
    public string SelectedColor { get; set; } = string.Empty;

    [Required]
    public string SelectedSize { get; set; } = string.Empty;
}

public class UpdateCartItemRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

