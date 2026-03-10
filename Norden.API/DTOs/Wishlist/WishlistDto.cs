using Norden.API.DTOs.Common;

namespace Norden.API.DTOs.Wishlist
{
    public class WishlistItemDto
    {
        public string Id { get; set; } = string.Empty;
        public ProductSummaryDto Product { get; set; } = new ProductSummaryDto();
        public DateTime CreatedAt { get; set; }
    }

    public class ProductSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Image { get; set; }
    }

    public class WishlistDto
    {
        public List<WishlistItemDto> Items { get; set; } = new List<WishlistItemDto>();
    }

    public class AddToWishlistRequest
    {
        public string ProductId { get; set; } = string.Empty;
    }
}
