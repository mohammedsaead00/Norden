using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Norden.API.Data;
using Norden.API.DTOs.Cart;
using Norden.API.DTOs.Common;
using Norden.API.Models;
using System.Security.Claims;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(AppDbContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<Cart?> GetOrCreateCart(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, UpdatedAt = DateTime.UtcNow };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(ApiResponse<CartDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));

                var cart = await GetOrCreateCart(userId.Value);

                var cartItems = cart!.CartItems.Select(ci => new CartItemDto
                {
                    Id = ci.Id.ToString(),
                    Product = new ProductSummaryDto
                    {
                        Id = ci.Product.Id.ToString(),
                        Name = ci.Product.Name,
                        Price = ci.Product.Price,
                        Image = ci.Product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.ImageUrl
                    },
                    Quantity = ci.Quantity,
                    SelectedColor = ci.SelectedColor,
                    SelectedSize = ci.SelectedSize
                }).ToList();

                var subtotal = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);
                var totalItems = cartItems.Sum(ci => ci.Quantity);

                var result = new CartDto
                {
                    Items = cartItems,
                    Subtotal = subtotal,
                    TotalItems = totalItems
                };

                return Ok(ApiResponse<CartDto>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart");
                return StatusCode(500, ApiResponse<CartDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> AddToCart(AddToCartRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<CartItemDto>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));

                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(ApiResponse<CartItemDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));

                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.ProductId) && p.IsActive);

                if (product == null)
                    return NotFound(ApiResponse<CartItemDto>.ErrorResult("Product not found", "NOT_FOUND"));

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId.Value);

                if (cart == null)
                {
                    cart = new Cart { UserId = userId.Value, UpdatedAt = DateTime.UtcNow };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var existingItem = cart.CartItems.FirstOrDefault(ci =>
                    ci.ProductId == Guid.Parse(request.ProductId) &&
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
                        ProductId = Guid.Parse(request.ProductId),
                        Quantity = request.Quantity,
                        SelectedColor = request.SelectedColor,
                        SelectedSize = request.SelectedSize,
                        AddedAt = DateTime.UtcNow
                    };
                    _context.CartItems.Add(cartItem);
                    existingItem = cartItem;
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var cartItemDto = new CartItemDto
                {
                    Id = existingItem.Id.ToString(),
                    Product = new ProductSummaryDto
                    {
                        Id = product.Id.ToString(),
                        Name = product.Name,
                        Price = product.Price,
                        Image = product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.ImageUrl
                    },
                    Quantity = existingItem.Quantity,
                    SelectedColor = request.SelectedColor,
                    SelectedSize = request.SelectedSize
                };

                return Ok(ApiResponse<CartItemDto>.SuccessResult(cartItemDto, "Item added to cart"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return StatusCode(500, ApiResponse<CartItemDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPut("items/{itemId}")]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> UpdateCartItem(Guid itemId, UpdateCartItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<CartItemDto>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));

                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(ApiResponse<CartItemDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
                    .FirstOrDefaultAsync(c => c.UserId == userId.Value);

                if (cart == null)
                    return NotFound(ApiResponse<CartItemDto>.ErrorResult("Cart not found", "NOT_FOUND"));

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == itemId);
                if (cartItem == null)
                    return NotFound(ApiResponse<CartItemDto>.ErrorResult("Cart item not found", "NOT_FOUND"));

                cartItem.Quantity = request.Quantity;
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var cartItemDto = new CartItemDto
                {
                    Id = cartItem.Id.ToString(),
                    Product = new ProductSummaryDto
                    {
                        Id = cartItem.Product.Id.ToString(),
                        Name = cartItem.Product.Name,
                        Price = cartItem.Product.Price,
                        Image = cartItem.Product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.ImageUrl
                    },
                    Quantity = cartItem.Quantity,
                    SelectedColor = cartItem.SelectedColor,
                    SelectedSize = cartItem.SelectedSize
                };

                return Ok(ApiResponse<CartItemDto>.SuccessResult(cartItemDto, "Cart item updated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {ItemId}", itemId);
                return StatusCode(500, ApiResponse<CartItemDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpDelete("items/{itemId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveFromCart(Guid itemId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(ApiResponse<object>.ErrorResult("Unauthorized", "UNAUTHORIZED"));

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId.Value);

                if (cart == null)
                    return NotFound(ApiResponse<object>.ErrorResult("Cart not found", "NOT_FOUND"));

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == itemId);
                if (cartItem == null)
                    return NotFound(ApiResponse<object>.ErrorResult("Cart item not found", "NOT_FOUND"));

                _context.CartItems.Remove(cartItem);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(null, "Item removed from cart"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {ItemId}", itemId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<object>>> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(ApiResponse<object>.ErrorResult("Unauthorized", "UNAUTHORIZED"));

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId.Value);

                if (cart == null)
                    return Ok(ApiResponse<object>.SuccessResult(null, "Cart already empty"));

                _context.CartItems.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(null, "Cart cleared"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
    }
}
