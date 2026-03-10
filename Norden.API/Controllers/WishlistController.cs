using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Norden.API.Data;
using Norden.API.DTOs.Common;
using Norden.API.DTOs.Wishlist;
using Norden.API.Models;
using System.Security.Claims;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(AppDbContext context, ILogger<WishlistController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<WishlistDto>>> GetWishlist()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<WishlistDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var wishlistItems = await _context.WishlistItems
                    .Include(wi => wi.Product)
                    .ThenInclude(p => p.Images)
                    .Where(wi => wi.UserId == userId.Value)
                    .Select(wi => new WishlistItemDto
                    {
                        Id = wi.Id.ToString(),
                        Product = new ProductSummaryDto
                        {
                            Id = wi.Product.Id.ToString(),
                            Name = wi.Product.Name,
                            Price = wi.Product.Price,
                            Image = wi.Product.Images.OrderBy(i => i.SortOrder).FirstOrDefault() != null 
                                ? wi.Product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()!.ImageUrl 
                                : null
                        },
                        CreatedAt = wi.CreatedAt
                    })
                    .ToListAsync();

                var result = new WishlistDto
                {
                    Items = wishlistItems
                };

                return Ok(ApiResponse<WishlistDto>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist");
                return StatusCode(500, ApiResponse<WishlistDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<WishlistItemDto>>> AddToWishlist(AddToWishlistRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<WishlistItemDto>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<WishlistItemDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                // Check if product exists
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.ProductId) && p.IsActive);

                if (product == null)
                {
                    return NotFound(ApiResponse<WishlistItemDto>.ErrorResult("Product not found", "NOT_FOUND"));
                }

                // Check if item already exists in wishlist
                var existingItem = await _context.WishlistItems
                    .FirstOrDefaultAsync(wi => wi.UserId == userId.Value && wi.ProductId == Guid.Parse(request.ProductId));

                if (existingItem != null)
                {
                    return BadRequest(ApiResponse<WishlistItemDto>.ErrorResult("Item already in wishlist", "DUPLICATE_ITEM"));
                }

                var wishlistItem = new WishlistItem
                {
                    UserId = userId.Value,
                    ProductId = Guid.Parse(request.ProductId),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WishlistItems.Add(wishlistItem);
                await _context.SaveChangesAsync();

                var wishlistItemDto = new WishlistItemDto
                {
                    Id = wishlistItem.Id.ToString(),
                    Product = new ProductSummaryDto
                    {
                        Id = product.Id.ToString(),
                        Name = product.Name,
                        Price = product.Price,
                        Image = product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.ImageUrl
                    },
                    CreatedAt = wishlistItem.CreatedAt
                };

                return Ok(ApiResponse<WishlistItemDto>.SuccessResult(wishlistItemDto, "Item added to wishlist"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to wishlist");
                return StatusCode(500, ApiResponse<WishlistItemDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveFromWishlist(Guid productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var wishlistItem = await _context.WishlistItems
                    .FirstOrDefaultAsync(wi => wi.UserId == userId.Value && wi.ProductId == productId);

                if (wishlistItem == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Wishlist item not found", "NOT_FOUND"));
                }

                _context.WishlistItems.Remove(wishlistItem);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(null, "Item removed from wishlist"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing wishlist item {ProductId}", productId);
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
