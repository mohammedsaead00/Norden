using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Norden.API.Data;
using Norden.API.DTOs.Common;
using Norden.API.DTOs.Reviews;
using Norden.API.Models;
using System.Security.Claims;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(AppDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("products/{productId}")]
        public async Task<ActionResult<ApiResponse<ReviewListDto>>> GetProductReviews(Guid productId, [FromQuery] ReviewQueryDto query)
        {
            try
            {
                var queryable = _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.ProductId == productId);

                if (query.Rating.HasValue)
                    queryable = queryable.Where(r => r.Rating == query.Rating.Value);

                // Apply sorting
                queryable = query.SortBy?.ToLower() switch
                {
                    "date" => queryable.OrderByDescending(r => r.CreatedAt),
                    "helpful" => queryable.OrderByDescending(r => r.HelpfulCount),
                    "rating" => queryable.OrderByDescending(r => r.Rating),
                    _ => queryable.OrderByDescending(r => r.CreatedAt)
                };

                var totalItems = await queryable.CountAsync();

                var reviews = await queryable
                    .Skip((query.Page - 1) * query.Limit)
                    .Take(query.Limit)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id.ToString(),
                        UserId = r.UserId.ToString(),
                        UserName = r.User.Name ?? r.User.Email,
                        UserImageUrl = r.User.AvatarUrl,
                        Rating = r.Rating,
                        Title = r.Title,
                        Comment = r.Comment,
                        Images = r.GetImages(),
                        IsVerified = r.IsVerified,
                        HelpfulCount = r.HelpfulCount,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                var result = new ReviewListDto
                {
                    Reviews = reviews,
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = query.Page,
                        TotalItems = totalItems,
                        ItemsPerPage = query.Limit,
                        TotalPages = (int)Math.Ceiling((double)totalItems / query.Limit)
                    }
                };

                return Ok(ApiResponse<ReviewListDto>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for product {ProductId}", productId);
                return StatusCode(500, ApiResponse<ReviewListDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPost("products/{productId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> CreateReview(Guid productId, CreateReviewRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<ReviewDto>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<ReviewDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                // Check if user already reviewed this product
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId.Value);

                if (existingReview != null)
                {
                    return BadRequest(ApiResponse<ReviewDto>.ErrorResult("You have already reviewed this product", "DUPLICATE_REVIEW"));
                }

                var review = new Review
                {
                    ProductId = productId,
                    UserId = userId.Value,
                    Rating = request.Rating,
                    Title = request.Title,
                    Comment = request.Comment,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                if (request.Images != null && request.Images.Any())
                {
                    review.SetImages(request.Images);
                }

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                var reviewDto = new ReviewDto
                {
                    Id = review.Id.ToString(),
                    UserId = review.UserId.ToString(),
                    Rating = review.Rating,
                    Title = review.Title,
                    Comment = review.Comment,
                    Images = review.GetImages(),
                    IsVerified = review.IsVerified,
                    HelpfulCount = review.HelpfulCount,
                    CreatedAt = review.CreatedAt
                };

                return Ok(ApiResponse<ReviewDto>.SuccessResult(reviewDto, "Review created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for product {ProductId}", productId);
                return StatusCode(500, ApiResponse<ReviewDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPut("{reviewId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> UpdateReview(Guid reviewId, UpdateReviewRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<ReviewDto>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<ReviewDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId.Value);

                if (review == null)
                {
                    return NotFound(ApiResponse<ReviewDto>.ErrorResult("Review not found", "NOT_FOUND"));
                }

                review.Rating = request.Rating;
                review.Title = request.Title;
                review.Comment = request.Comment;
                review.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var reviewDto = new ReviewDto
                {
                    Id = review.Id.ToString(),
                    UserId = review.UserId.ToString(),
                    Rating = review.Rating,
                    Title = review.Title,
                    Comment = review.Comment,
                    Images = review.GetImages(),
                    IsVerified = review.IsVerified,
                    HelpfulCount = review.HelpfulCount,
                    CreatedAt = review.CreatedAt
                };

                return Ok(ApiResponse<ReviewDto>.SuccessResult(reviewDto, "Review updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {ReviewId}", reviewId);
                return StatusCode(500, ApiResponse<ReviewDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpDelete("{reviewId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> DeleteReview(Guid reviewId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId.Value);

                if (review == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Review not found", "NOT_FOUND"));
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(null, "Review deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", reviewId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPost("{reviewId}/helpful")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ReviewHelpfulResponse>>> MarkReviewHelpful(Guid reviewId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<ReviewHelpfulResponse>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var review = await _context.Reviews
                    .Include(r => r.ReviewHelpfuls)
                    .FirstOrDefaultAsync(r => r.Id == reviewId);

                if (review == null)
                {
                    return NotFound(ApiResponse<ReviewHelpfulResponse>.ErrorResult("Review not found", "NOT_FOUND"));
                }

                var existingHelpful = review.ReviewHelpfuls.FirstOrDefault(h => h.UserId == userId.Value);
                if (existingHelpful != null)
                {
                    // Remove helpful
                    _context.ReviewHelpfuls.Remove(existingHelpful);
                    review.HelpfulCount--;
                }
                else
                {
                    // Add helpful
                    var helpful = new ReviewHelpful
                    {
                        ReviewId = reviewId,
                        UserId = userId.Value,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.ReviewHelpfuls.Add(helpful);
                    review.HelpfulCount++;
                }

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<ReviewHelpfulResponse>.SuccessResult(new ReviewHelpfulResponse
                {
                    HelpfulCount = review.HelpfulCount
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking review helpful {ReviewId}", reviewId);
                return StatusCode(500, ApiResponse<ReviewHelpfulResponse>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
    }
}
