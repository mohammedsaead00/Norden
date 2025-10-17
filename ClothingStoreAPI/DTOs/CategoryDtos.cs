using System.ComponentModel.DataAnnotations;

namespace ClothingStoreAPI.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public string Gender { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCategoryDto
{
    [Required]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;
}

public class UpdateCategoryDto
{
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? Gender { get; set; }
    public bool? IsActive { get; set; }
}

