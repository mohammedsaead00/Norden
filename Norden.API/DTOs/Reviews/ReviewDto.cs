using Norden.API.DTOs.Common;

namespace Norden.API.DTOs.Reviews
{
    public class ReviewDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserImageUrl { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
        public bool IsVerified { get; set; }
        public int HelpfulCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewRequest
    {
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<string>? Images { get; set; }
    }

    public class UpdateReviewRequest
    {
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewListDto
    {
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    public class ReviewQueryDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public int? Rating { get; set; }
        public string? SortBy { get; set; }
    }

    public class ReviewHelpfulResponse
    {
        public int HelpfulCount { get; set; }
    }
}
