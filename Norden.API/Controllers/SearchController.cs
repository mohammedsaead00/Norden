using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Norden.API.Data;
using Norden.API.DTOs.Common;
using Norden.API.DTOs.Search;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SearchController> _logger;

        public SearchController(AppDbContext context, ILogger<SearchController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<SearchResponse>>> Search([FromQuery] SearchRequest request)
        {
            try
            {
                var queryable = _context.Products
                    .Include(p => p.Reviews)
                    .Include(p => p.Category)
                    .Include(p => p.Season)
                    .Include(p => p.Images)
                    .Where(p => p.IsActive);

                // Apply search query
                if (!string.IsNullOrEmpty(request.Q))
                {
                    queryable = queryable.Where(p => p.Name.Contains(request.Q) || 
                                                   (p.Description != null && p.Description.Contains(request.Q)) ||
                                                   (p.Category != null && p.Category.Name.Contains(request.Q)));
                }

                // Apply filters
                if (!string.IsNullOrEmpty(request.Category))
                    queryable = queryable.Where(p => p.Category != null && p.Category.Slug == request.Category);

                if (request.MinPrice.HasValue)
                    queryable = queryable.Where(p => p.Price >= request.MinPrice.Value);

                if (request.MaxPrice.HasValue)
                    queryable = queryable.Where(p => p.Price <= request.MaxPrice.Value);

                if (request.Rating.HasValue)
                    queryable = queryable.Where(p => p.Reviews.Any(r => r.Rating >= request.Rating.Value));

                // Apply sorting
                queryable = request.SortBy?.ToLower() switch
                {
                    "price" => request.SortOrder == "desc" ? queryable.OrderByDescending(p => p.Price) : queryable.OrderBy(p => p.Price),
                    "rating" => request.SortOrder == "desc" ? queryable.OrderByDescending(p => p.Reviews.Average(r => r.Rating)) : queryable.OrderBy(p => p.Reviews.Average(r => r.Rating)),
                    "date" => request.SortOrder == "desc" ? queryable.OrderByDescending(p => p.CreatedAt) : queryable.OrderBy(p => p.CreatedAt),
                    _ => queryable.OrderBy(p => p.Name)
                };

                // Get total count
                var totalItems = await queryable.CountAsync();

                // Apply pagination
                var pagedProducts = await queryable
                    .Skip((request.Page - 1) * request.Limit)
                    .Take(request.Limit)
                    .ToListAsync();
                    
                var products = pagedProducts.Select(p => new ProductDto
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Category = p.Category?.Name,
                    Season = p.Season?.Slug ?? p.Category?.Season,
                    Images = p.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).ToList(),
                    IsNew = p.IsNew,
                    IsFeatured = p.IsFeatured,
                    Rating = p.Reviews.Any() ? new ProductRatingDto
                    {
                        AverageRating = p.Reviews.Average(r => r.Rating),
                        TotalReviews = p.Reviews.Count
                    } : null
                }).ToList();

                // Get filter options
                var allProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .Where(p => p.IsActive)
                    .ToListAsync();

                var categories = allProducts
                    .Where(p => p.Category != null)
                    .Select(p => p.Category!.Slug)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                var brands = new List<string>(); // Brands removed from new model

                var priceRange = allProducts.Any() ? new PriceRange
                {
                    Min = allProducts.Min(p => p.Price),
                    Max = allProducts.Max(p => p.Price)
                } : null;

                var ratings = allProducts
                    .Where(p => p.Reviews.Any())
                    .Select(p => (int)Math.Round(p.Reviews.Average(r => r.Rating)))
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();

                var result = new SearchResponse
                {
                    Products = products,
                    Filters = new SearchFilters
                    {
                        Categories = categories,
                        Brands = brands,
                        PriceRange = priceRange,
                        Ratings = ratings
                    },
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = request.Page,
                        TotalItems = totalItems,
                        ItemsPerPage = request.Limit,
                        TotalPages = (int)Math.Ceiling((double)totalItems / request.Limit)
                    }
                };

                return Ok(ApiResponse<SearchResponse>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing search");
                return StatusCode(500, ApiResponse<SearchResponse>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }
    }
}
