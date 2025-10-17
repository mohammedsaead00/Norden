using System.ComponentModel.DataAnnotations;

namespace ClothingStoreAPI.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public int ShippingAddressId { get; set; }
    public int BillingAddressId { get; set; }
    public string? TrackingNumber { get; set; }
    public List<OrderItemDto>? OrderItems { get; set; }
    public PaymentDto? Payment { get; set; }
}

public class CreateOrderDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int ShippingAddressId { get; set; }

    [Required]
    public int BillingAddressId { get; set; }

    [Required]
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

public class UpdateOrderDto
{
    public string? OrderStatus { get; set; }
    public string? TrackingNumber { get; set; }
}

public class OrderItemDto
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int InventoryId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class CreateOrderItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int InventoryId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

