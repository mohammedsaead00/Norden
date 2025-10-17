using System.ComponentModel.DataAnnotations;

namespace ClothingStoreAPI.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal BasePrice { get; set; }
    public string? Brand { get; set; }
    public string? Material { get; set; }
    public string? CareInstructions { get; set; }
    public bool IsActive { get; set; }
    public List<ProductImageDto>? Images { get; set; }
    public List<InventoryDto>? InventoryItems { get; set; }
}

public class CreateProductDto
{
    [Required]
    public string ProductName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal BasePrice { get; set; }

    public string? Brand { get; set; }
    public string? Material { get; set; }
    public string? CareInstructions { get; set; }
}

public class UpdateProductDto
{
    public string? ProductName { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public decimal? BasePrice { get; set; }
    public string? Brand { get; set; }
    public string? Material { get; set; }
    public string? CareInstructions { get; set; }
    public bool? IsActive { get; set; }
}

public class ProductImageDto
{
    public int ImageId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateProductImageDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

