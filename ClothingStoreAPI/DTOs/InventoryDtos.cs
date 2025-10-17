using System.ComponentModel.DataAnnotations;

namespace ClothingStoreAPI.DTOs;

public class InventoryDto
{
    public int InventoryId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; }
    public DateTime? LastRestocked { get; set; }
}

public class CreateInventoryDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public string Size { get; set; } = string.Empty;

    [Required]
    public string Color { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; set; }

    public int ReorderLevel { get; set; } = 10;
}

public class UpdateInventoryDto
{
    public int? QuantityInStock { get; set; }
    public int? ReorderLevel { get; set; }
    public DateTime? LastRestocked { get; set; }
}

