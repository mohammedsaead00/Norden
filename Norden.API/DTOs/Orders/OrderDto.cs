using Norden.API.DTOs.Common;

namespace Norden.API.DTOs.Orders
{
    public class OrderDto
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public AddressInfo Address { get; set; } = new AddressInfo();
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? SelectedColor { get; set; }
        public string? SelectedSize { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class AddressInfo
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class CreateOrderRequest
    {
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
        public AddressInfo ShippingAddress { get; set; } = new AddressInfo();
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class OrderItemRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? SelectedColor { get; set; }
        public string? SelectedSize { get; set; }
    }

    public class OrderListDto
    {
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    public class OrderQueryDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? Status { get; set; }
    }

    public class CreateOrderResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OrderTrackingDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
