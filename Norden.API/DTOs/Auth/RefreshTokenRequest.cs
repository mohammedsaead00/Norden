using System.ComponentModel.DataAnnotations;

namespace Norden.API.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
