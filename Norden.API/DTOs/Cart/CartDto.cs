using Norden.API.DTOs.Common;

namespace Norden.API.DTOs.Cart
{
    public class CartItemDto
    {
        public string Id { get; set; } = string.Empty;
        public ProductSummaryDto Product { get; set; } = new ProductSummaryDto();
        public int Quantity { get; set; }
        public string? SelectedColor { get; set; }
        public string? SelectedSize { get; set; }
    }

    public class ProductSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Image { get; set; }
    }

    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal Subtotal { get; set; }
        public int TotalItems { get; set; }
    }

    public class AddToCartRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public string? SelectedColor { get; set; }
        public string? SelectedSize { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }
}
