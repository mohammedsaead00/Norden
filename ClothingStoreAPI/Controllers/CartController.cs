using Microsoft.AspNetCore.Mvc;
using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Services;

namespace ClothingStoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Get user's cart items
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<CartDto>>> GetUserCart(int userId)
    {
        var cartItems = await _cartService.GetUserCartAsync(userId);
        return Ok(cartItems);
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartDto addToCartDto)
    {
        var cartItem = await _cartService.AddToCartAsync(addToCartDto);
        
        if (cartItem == null)
        {
            return BadRequest(new { message = "Failed to add item to cart. Check product availability." });
        }

        return Ok(cartItem);
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    [HttpPut("{cartId}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(int cartId, [FromBody] UpdateCartDto updateDto)
    {
        var cartItem = await _cartService.UpdateCartItemAsync(cartId, updateDto);
        
        if (cartItem == null)
        {
            return NotFound(new { message = "Cart item not found or insufficient stock" });
        }

        return Ok(cartItem);
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("{cartId}")]
    public async Task<ActionResult> RemoveFromCart(int cartId)
    {
        var result = await _cartService.RemoveFromCartAsync(cartId);
        
        if (!result)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Clear user's cart
    /// </summary>
    [HttpDelete("user/{userId}")]
    public async Task<ActionResult> ClearCart(int userId)
    {
        await _cartService.ClearCartAsync(userId);
        return NoContent();
    }
}

