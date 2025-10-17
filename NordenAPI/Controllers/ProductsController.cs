using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NordenAPI.Data;
using NordenAPI.DTOs;
using NordenAPI.Models;
using System.Text.Json;

namespace NordenAPI.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly NordenDbContext _context;

    public ProductsController(NordenDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all products with filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ProductListResponse>>> GetAll(
        [FromQuery] string? category,
        [FromQuery] bool? isFeatured,
        [FromQuery] bool? isNew,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(category) && Enum.TryParse<ProductCategory>(category, true, out var categoryEnum))
        {
            query = query.Where(p => p.Category == categoryEnum);
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == isFeatured.Value);
        }

        if (isNew.HasValue)
        {
            query = query.Where(p => p.IsNew == isNew.Value);
        }

        var total = await query.CountAsync();
        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Category = p.Category.ToString(),
            Images = JsonSerializer.Deserialize<List<string>>(p.Images) ?? new List<string>(),
            Colors = JsonSerializer.Deserialize<List<string>>(p.Colors) ?? new List<string>(),
            Sizes = JsonSerializer.Deserialize<List<string>>(p.Sizes) ?? new List<string>(),
            Stock = p.Stock,
            IsNew = p.IsNew,
            IsFeatured = p.IsFeatured,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        var response = new ProductListResponse
        {
            Products = productDtos,
            Total = total,
            Limit = limit,
            Offset = offset
        };

        return Ok(ApiResponse<ProductListResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(string id)
    {
        if (!Guid.TryParse(id, out var productId))
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse(
                "INVALID_ID",
                "Invalid product ID format"
            ));
        }

        var product = await _context.Products.FindAsync(productId);

        if (product == null)
        {
            return NotFound(ApiResponse<ProductDto>.ErrorResponse(
                "NOT_FOUND",
                "Product not found"
            ));
        }

        var productDto = new ProductDto
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category.ToString(),
            Images = JsonSerializer.Deserialize<List<string>>(product.Images) ?? new List<string>(),
            Colors = JsonSerializer.Deserialize<List<string>>(product.Colors) ?? new List<string>(),
            Sizes = JsonSerializer.Deserialize<List<string>>(product.Sizes) ?? new List<string>(),
            Stock = product.Stock,
            IsNew = product.IsNew,
            IsFeatured = product.IsFeatured,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return Ok(ApiResponse<ProductDto>.SuccessResponse(productDto));
    }

    /// <summary>
    /// Search products
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<ProductListResponse>>> Search(
        [FromQuery] string q,
        [FromQuery] string? category,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(ApiResponse<ProductListResponse>.ErrorResponse(
                "INVALID_QUERY",
                "Search query is required"
            ));
        }

        var query = _context.Products
            .Where(p => p.Name.Contains(q) || p.Description.Contains(q));

        if (!string.IsNullOrEmpty(category) && Enum.TryParse<ProductCategory>(category, true, out var categoryEnum))
        {
            query = query.Where(p => p.Category == categoryEnum);
        }

        var total = await query.CountAsync();
        var products = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Category = p.Category.ToString(),
            Images = JsonSerializer.Deserialize<List<string>>(p.Images) ?? new List<string>(),
            Colors = JsonSerializer.Deserialize<List<string>>(p.Colors) ?? new List<string>(),
            Sizes = JsonSerializer.Deserialize<List<string>>(p.Sizes) ?? new List<string>(),
            Stock = p.Stock,
            IsNew = p.IsNew,
            IsFeatured = p.IsFeatured,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        var response = new ProductListResponse
        {
            Products = productDtos,
            Total = total,
            Limit = limit,
            Offset = offset
        };

        return Ok(ApiResponse<ProductListResponse>.SuccessResponse(response));
    }
}

