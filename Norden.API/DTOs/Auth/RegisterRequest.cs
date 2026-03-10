using System.ComponentModel.DataAnnotations;

namespace Norden.API.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Name { get; set; }

        [Phone]
        public string? Phone { get; set; }
    }
}
