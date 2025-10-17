using System.ComponentModel.DataAnnotations;

namespace ClothingStoreAPI.DTOs;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    public string? ReviewText { get; set; }
}

public class UpdateReviewDto
{
    [Range(1, 5)]
    public int? Rating { get; set; }

    public string? ReviewText { get; set; }
}

