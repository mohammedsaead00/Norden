using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Norden.API.Data;
using Norden.API.DTOs.Common;
using Norden.API.DTOs.Products;
using Norden.API.Models;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ProductListDto>>> GetProducts([FromQuery] ProductQueryDto query)
        {
            try
            {
                var queryable = _context.Products
                    .Include(p => p.Reviews)
                    .Include(p => p.Category)
                    .Include(p => p.Season)
                    .Include(p => p.Images)
                    .Include(p => p.Variants)
                    .Where(p => p.IsActive);

                // Apply filters
                if (!string.IsNullOrEmpty(query.Category))
                    queryable = queryable.Where(p => p.Category != null && p.Category.Slug == query.Category);

                // Note: According to API Spec, Season can be 'winter', 'summer', or 'all'.
                if (!string.IsNullOrEmpty(query.Season) && query.Season != "all")
                {
                    // If a specific season is requested, we show products for that season AND products marked for 'all' seasons if that logic was required. 
                    // Based on spec: "When a user browses a season, the query should return products where season = <selected> OR season = 'all'."
                    queryable = queryable.Where(p => 
                        p.Season != null && (p.Season.Slug == query.Season || p.Season.Slug == "all") ||
                        (p.Category != null && p.Category.Season == "all")
                    );
                }

                // If brand exists in query (not in spec but keeping if frontend sends it)
                if (!string.IsNullOrEmpty(query.Brand))
                {
                    // No Brand property on new model, this is safe to ignore or we can search in description
                    // queryable = queryable.Where(p => p.Description.Contains(query.Brand));
                }

                if (query.MinPrice.HasValue)
                    queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);

                if (query.MaxPrice.HasValue)
                    queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);

                if (!string.IsNullOrEmpty(query.Search))
                    queryable = queryable.Where(p => p.Name.Contains(query.Search) || 
                                                   (p.Description != null && p.Description.Contains(query.Search)));

                // Spec additional filters
                if (query.IsFeatured.HasValue && query.IsFeatured.Value)
                    queryable = queryable.Where(p => p.IsFeatured);
                    
                if (query.IsNew.HasValue && query.IsNew.Value)
                    queryable = queryable.Where(p => p.IsNew);

                // Apply sorting
                queryable = query.SortBy?.ToLower() switch
                {
                    "price" => query.SortOrder == "desc" ? queryable.OrderByDescending(p => p.Price) : queryable.OrderBy(p => p.Price),
                    "rating" => query.SortOrder == "desc" ? queryable.OrderByDescending(p => p.Reviews.Average(r => r.Rating)) : queryable.OrderBy(p => p.Reviews.Average(r => r.Rating)),
                    "date" => query.SortOrder == "desc" ? queryable.OrderByDescending(p => p.CreatedAt) : queryable.OrderBy(p => p.CreatedAt),
                    _ => queryable.OrderBy(p => p.CreatedAt) // Default to latest
                };

                // Get total count
                var totalItems = await queryable.CountAsync();

                // Apply pagination
                var pagedProducts = await queryable
                    .Skip((query.Page - 1) * query.Limit)
                    .Take(query.Limit)
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
                    Colors = p.Variants.Select(v => v.Color).Distinct().ToList(),
                    Sizes = p.Variants.Select(v => v.Size).Distinct().ToList(),
                    IsNew = p.IsNew,
                    IsFeatured = p.IsFeatured,
                    Stock = p.Stock,
                    Rating = p.Reviews.Any() ? Math.Round(p.Reviews.Average(r => r.Rating), 1) : 0,
                    ReviewCount = p.Reviews.Count,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                var result = new ProductListDto
                {
                    Products = products,
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = query.Page,
                        TotalItems = totalItems,
                        ItemsPerPage = query.Limit,
                        TotalPages = (int)Math.Ceiling((double)totalItems / query.Limit)
                    }
                };

                return Ok(ApiResponse<ProductListDto>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, ApiResponse<ProductListDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(Guid id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Reviews)
                    .Include(p => p.Category)
                    .Include(p => p.Season)
                    .Include(p => p.Images)
                    .Include(p => p.Variants)
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

                if (product == null)
                {
                    return NotFound(ApiResponse<ProductDto>.ErrorResult("Product not found", "NOT_FOUND"));
                }

                var productDto = new ProductDto
                {
                    Id = product.Id.ToString(),
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = product.Category?.Name,
                    Season = product.Season?.Slug ?? product.Category?.Season,
                    Images = product.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).ToList(),
                    Colors = product.Variants.Select(v => v.Color).Distinct().ToList(),
                    Sizes = product.Variants.Select(v => v.Size).Distinct().ToList(),
                    IsNew = product.IsNew,
                    IsFeatured = product.IsFeatured,
                    Stock = product.Stock,
                    Rating = product.Reviews.Any() ? Math.Round(product.Reviews.Average(r => r.Rating), 1) : 0,
                    ReviewCount = product.Reviews.Count,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return Ok(ApiResponse<ProductDto>.SuccessResult(productDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId}", id);
                return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }
    }
}
