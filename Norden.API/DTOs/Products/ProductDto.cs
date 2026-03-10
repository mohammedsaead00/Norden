using Norden.API.DTOs.Common;

namespace Norden.API.DTOs.Products
{
    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? Season { get; set; }
        
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Colors { get; set; } = new List<string>();
        public List<string> Sizes { get; set; } = new List<string>();
        
        public bool IsNew { get; set; }
        public bool IsFeatured { get; set; }
        public int Stock { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductRatingDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int>? RatingDistribution { get; set; }
    }

    public class ProductListDto
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    public class ProductQueryDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Category { get; set; }
        public string? Season { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? Rating { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsNew { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
        public string? Search { get; set; }
    }
}
