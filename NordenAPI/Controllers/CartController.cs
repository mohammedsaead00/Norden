using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NordenAPI.Data;
using NordenAPI.DTOs;
using NordenAPI.Models;
using System.Security.Claims;
using System.Text.Json;

namespace NordenAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly NordenDbContext _context;

    public CartController(NordenDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Get user cart
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
    {
        var userId = GetUserId();
        
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var cartDto = new CartDto
        {
            Items = cart.Items.Select(ci => new CartItemDto
            {
                Id = ci.Id.ToString(),
                ProductId = ci.ProductId.ToString(),
                ProductName = ci.Product?.Name ?? "",
                ProductImage = ci.Product != null 
                    ? JsonSerializer.Deserialize<List<string>>(ci.Product.Images)?.FirstOrDefault() ?? "" 
                    : "",
                Price = ci.Price,
                Quantity = ci.Quantity,
                SelectedColor = ci.SelectedColor,
                SelectedSize = ci.SelectedSize,
                AddedAt = ci.AddedAt
            }).ToList(),
            Subtotal = cart.Items.Sum(ci => ci.Price * ci.Quantity),
            Tax = 0,
            Shipping = 0,
            Total = cart.Items.Sum(ci => ci.Price * ci.Quantity)
        };

        return Ok(ApiResponse<CartDto>.SuccessResponse(cartDto));
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> AddToCart([FromBody] AddToCartRequest request)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(request.ProductId, out var productId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid product ID"
            ));
        }

        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.Stock < request.Quantity)
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INSUFFICIENT_STOCK", "Product not available or insufficient stock"
            ));
        }

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingItem = cart.Items.FirstOrDefault(ci => 
            ci.ProductId == productId && 
            ci.SelectedColor == request.SelectedColor && 
            ci.SelectedSize == request.SelectedSize);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = request.Quantity,
                SelectedColor = request.SelectedColor,
                SelectedSize = request.SelectedSize,
                Price = product.Price
            };
            _context.CartItems.Add(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Item added to cart" }
        ));
    }

    /// <summary>
    /// Update cart item
    /// </summary>
    [HttpPut("items/{cartItemId}")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> UpdateCartItem(
        string cartItemId, 
        [FromBody] UpdateCartItemRequest request)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(cartItemId, out var itemId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid cart item ID"
            ));
        }

        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart!.UserId == userId);

        if (cartItem == null)
        {
            return NotFound(ApiResponse<MessageResponse>.ErrorResponse(
                "NOT_FOUND", "Cart item not found"
            ));
        }

        if (cartItem.Product!.Stock < request.Quantity)
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INSUFFICIENT_STOCK", "Insufficient stock"
            ));
        }

        cartItem.Quantity = request.Quantity;
        cartItem.Cart!.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Cart item updated" }
        ));
    }

    /// <summary>
    /// Remove from cart
    /// </summary>
    [HttpDelete("items/{cartItemId}")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> RemoveFromCart(string cartItemId)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(cartItemId, out var itemId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid cart item ID"
            ));
        }

        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart!.UserId == userId);

        if (cartItem == null)
        {
            return NotFound(ApiResponse<MessageResponse>.ErrorResponse(
                "NOT_FOUND", "Cart item not found"
            ));
        }

        _context.CartItems.Remove(cartItem);
        cartItem.Cart!.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Item removed from cart" }
        ));
    }

    /// <summary>
    /// Clear cart
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> ClearCart()
    {
        var userId = GetUserId();

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null)
        {
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
        }

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Cart cleared" }
        ));
    }
}

