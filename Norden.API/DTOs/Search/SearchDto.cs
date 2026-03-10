using Norden.API.DTOs.Common;

namespace Norden.API.DTOs.Search
{
    public class SearchRequest
    {
        public string? Q { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Rating { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
    }

    public class SearchResponse
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public SearchFilters Filters { get; set; } = new SearchFilters();
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    public class SearchFilters
    {
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Brands { get; set; } = new List<string>();
        public PriceRange? PriceRange { get; set; }
        public List<int> Ratings { get; set; } = new List<int>();
    }

    public class PriceRange
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }

    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? Season { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public bool IsNew { get; set; }
        public bool IsFeatured { get; set; }
        public ProductRatingDto? Rating { get; set; }
    }

    public class ProductRatingDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
