using System.ComponentModel.DataAnnotations;
using NordenAPI.Models;

namespace NordenAPI.DTOs;

public class CreateOrderRequest
{
    [Required]
    public string ShippingAddressId { get; set; } = string.Empty;

    [Required]
    public string PaymentMethod { get; set; } = "card"; // card or cash_on_delivery

    [Required]
    public List<OrderItemRequest> Items { get; set; } = new();

    [Required]
    public decimal Subtotal { get; set; }

    public decimal Tax { get; set; } = 0;
    public decimal Shipping { get; set; } = 0;

    [Required]
    public decimal Total { get; set; }
}

public class OrderItemRequest
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public string SelectedColor { get; set; } = string.Empty;

    [Required]
    public string SelectedSize { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }
}

public class OrderDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public AddressDto? ShippingAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string SelectedColor { get; set; } = string.Empty;
    public string SelectedSize { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}

public class OrderListResponse
{
    public List<OrderDto> Orders { get; set; } = new();
    public int Total { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}

public class CreateOrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

