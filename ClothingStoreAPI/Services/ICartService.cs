using ClothingStoreAPI.DTOs;

namespace ClothingStoreAPI.Services;

public interface ICartService
{
    Task<IEnumerable<CartDto>> GetUserCartAsync(int userId);
    Task<CartDto?> AddToCartAsync(AddToCartDto addToCartDto);
    Task<CartDto?> UpdateCartItemAsync(int cartId, UpdateCartDto updateDto);
    Task<bool> RemoveFromCartAsync(int cartId);
    Task<bool> ClearCartAsync(int userId);
}

