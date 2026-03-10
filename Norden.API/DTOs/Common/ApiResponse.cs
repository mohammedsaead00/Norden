namespace Norden.API.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ApiError? Error { get; set; }
        public string? Message { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> ErrorResult(string message, string? code = null, object? details = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = new ApiError
                {
                    Code = code,
                    Message = message,
                    Details = details
                }
            };
        }
    }

    public class ApiError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public object? Details { get; set; }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
