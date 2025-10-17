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
[Route("api/wishlist")]
public class WishlistController : ControllerBase
{
    private readonly NordenDbContext _context;

    public WishlistController(NordenDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Get user wishlist
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<WishlistDto>>> GetWishlist()
    {
        var userId = GetUserId();

        var wishlistItems = await _context.Wishlists
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .ToListAsync();

        var wishlistDto = new WishlistDto
        {
            Products = wishlistItems.Select(w => new WishlistItemDto
            {
                Id = w.Product!.Id.ToString(),
                Name = w.Product.Name,
                Price = w.Product.Price,
                Image = JsonSerializer.Deserialize<List<string>>(w.Product.Images)?.FirstOrDefault() ?? "",
                AddedAt = w.AddedAt
            }).ToList()
        };

        return Ok(ApiResponse<WishlistDto>.SuccessResponse(wishlistDto));
    }

    /// <summary>
    /// Add to wishlist
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> AddToWishlist([FromBody] AddToWishlistRequest request)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(request.ProductId, out var productId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid product ID"
            ));
        }

        // Check if already in wishlist
        var exists = await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        if (exists)
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "ALREADY_EXISTS", "Product already in wishlist"
            ));
        }

        var wishlistItem = new Wishlist
        {
            UserId = userId,
            ProductId = productId
        };

        _context.Wishlists.Add(wishlistItem);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Product added to wishlist" }
        ));
    }

    /// <summary>
    /// Remove from wishlist
    /// </summary>
    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> RemoveFromWishlist(string productId)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(productId, out var prodId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid product ID"
            ));
        }

        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == prodId);

        if (wishlistItem == null)
        {
            return NotFound(ApiResponse<MessageResponse>.ErrorResponse(
                "NOT_FOUND", "Product not in wishlist"
            ));
        }

        _context.Wishlists.Remove(wishlistItem);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Product removed from wishlist" }
        ));
    }

    /// <summary>
    /// Check if product in wishlist
    /// </summary>
    [HttpGet("check/{productId}")]
    public async Task<ActionResult<ApiResponse<WishlistCheckResponse>>> CheckWishlist(string productId)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(productId, out var prodId))
        {
            return BadRequest(ApiResponse<WishlistCheckResponse>.ErrorResponse(
                "INVALID_ID", "Invalid product ID"
            ));
        }

        var inWishlist = await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == prodId);

        return Ok(ApiResponse<WishlistCheckResponse>.SuccessResponse(
            new WishlistCheckResponse { InWishlist = inWishlist }
        ));
    }
}

