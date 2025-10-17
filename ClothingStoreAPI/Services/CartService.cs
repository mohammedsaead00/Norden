using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Models;
using ClothingStoreAPI.Repositories;
using ClothingStoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreAPI.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ClothingStoreContext _context;

    public CartService(ICartRepository cartRepository, ClothingStoreContext context)
    {
        _cartRepository = cartRepository;
        _context = context;
    }

    public async Task<IEnumerable<CartDto>> GetUserCartAsync(int userId)
    {
        var cartItems = await _cartRepository.GetByUserIdAsync(userId);
        return cartItems.Select(MapToDto);
    }

    public async Task<CartDto?> AddToCartAsync(AddToCartDto addToCartDto)
    {
        var product = await _context.Products.FindAsync(addToCartDto.ProductId);
        var inventory = await _context.Inventories.FindAsync(addToCartDto.InventoryId);

        if (product == null || inventory == null || inventory.QuantityInStock < addToCartDto.Quantity)
        {
            return null;
        }

        var cart = new Cart
        {
            UserId = addToCartDto.UserId,
            ProductId = addToCartDto.ProductId,
            InventoryId = addToCartDto.InventoryId,
            Quantity = addToCartDto.Quantity
        };

        var createdCart = await _cartRepository.AddAsync(cart);
        var cartWithDetails = await _context.Carts
            .Where(c => c.CartId == createdCart.CartId)
            .Select(c => new Cart
            {
                CartId = c.CartId,
                UserId = c.UserId,
                ProductId = c.ProductId,
                InventoryId = c.InventoryId,
                Quantity = c.Quantity,
                AddedAt = c.AddedAt,
                Product = c.Product,
                Inventory = c.Inventory
            })
            .FirstOrDefaultAsync();
        
        return cartWithDetails != null ? MapToDto(cartWithDetails) : null;
    }

    public async Task<CartDto?> UpdateCartItemAsync(int cartId, UpdateCartDto updateDto)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);
        if (cart == null)
        {
            return null;
        }

        var inventory = await _context.Inventories.FindAsync(cart.InventoryId);
        if (inventory == null || inventory.QuantityInStock < updateDto.Quantity)
        {
            return null;
        }

        cart.Quantity = updateDto.Quantity;

        await _cartRepository.UpdateAsync(cart);
        var updatedCart = await _context.Carts
            .Where(c => c.CartId == cartId)
            .Select(c => new Cart
            {
                CartId = c.CartId,
                UserId = c.UserId,
                ProductId = c.ProductId,
                InventoryId = c.InventoryId,
                Quantity = c.Quantity,
                AddedAt = c.AddedAt,
                Product = c.Product,
                Inventory = c.Inventory
            })
            .FirstOrDefaultAsync();
        
        return updatedCart != null ? MapToDto(updatedCart) : null;
    }

    public async Task<bool> RemoveFromCartAsync(int cartId)
    {
        if (!await _cartRepository.ExistsAsync(cartId))
        {
            return false;
        }

        await _cartRepository.DeleteAsync(cartId);
        return true;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        await _cartRepository.ClearUserCartAsync(userId);
        return true;
    }

    private CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            CartId = cart.CartId,
            UserId = cart.UserId,
            ProductId = cart.ProductId,
            ProductName = cart.Product?.ProductName,
            InventoryId = cart.InventoryId,
            Size = cart.Inventory?.Size,
            Color = cart.Inventory?.Color,
            Quantity = cart.Quantity,
            UnitPrice = cart.Product?.BasePrice ?? 0,
            Subtotal = (cart.Product?.BasePrice ?? 0) * cart.Quantity,
            AddedAt = cart.AddedAt
        };
    }
}

