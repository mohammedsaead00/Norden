namespace NordenAPI.DTOs;

public class WishlistDto
{
    public List<WishlistItemDto> Products { get; set; } = new();
}

public class WishlistItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}

public class AddToWishlistRequest
{
    public string ProductId { get; set; } = string.Empty;
}

public class WishlistCheckResponse
{
    public bool InWishlist { get; set; }
}

