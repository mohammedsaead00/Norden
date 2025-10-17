using Microsoft.AspNetCore.Mvc;
using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Models;
using ClothingStoreAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using ClothingStoreAPI.Data;

namespace ClothingStoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ClothingStoreContext _context;

    public CategoriesController(ClothingStoreContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _context.Categories
            .Include(c => c.ParentCategory)
            .Where(c => c.IsActive)
            .ToListAsync();

        var categoryDtos = categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            ParentCategoryId = c.ParentCategoryId,
            ParentCategoryName = c.ParentCategory?.CategoryName,
            Gender = c.Gender,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        });

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.CategoryId == id);
        
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        var categoryDto = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.CategoryName,
            Gender = category.Gender,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };

        return Ok(categoryDto);
    }

    /// <summary>
    /// Create new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto createDto)
    {
        var category = new Category
        {
            CategoryName = createDto.CategoryName,
            Description = createDto.Description,
            ParentCategoryId = createDto.ParentCategoryId,
            Gender = createDto.Gender
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var categoryDto = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            Gender = category.Gender,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, categoryDto);
    }

    /// <summary>
    /// Update category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryDto updateDto)
    {
        var category = await _context.Categories.FindAsync(id);
        
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        if (!string.IsNullOrEmpty(updateDto.CategoryName))
            category.CategoryName = updateDto.CategoryName;
        
        if (updateDto.Description != null)
            category.Description = updateDto.Description;
        
        if (updateDto.ParentCategoryId.HasValue)
            category.ParentCategoryId = updateDto.ParentCategoryId;
        
        if (!string.IsNullOrEmpty(updateDto.Gender))
            category.Gender = updateDto.Gender;
        
        if (updateDto.IsActive.HasValue)
            category.IsActive = updateDto.IsActive.Value;

        await _context.SaveChangesAsync();

        var categoryDto = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            Gender = category.Gender,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };

        return Ok(categoryDto);
    }

    /// <summary>
    /// Delete category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

