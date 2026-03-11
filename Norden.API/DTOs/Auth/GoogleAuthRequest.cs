using System.ComponentModel.DataAnnotations;

namespace Norden.API.DTOs.Auth
{
    public class GoogleAuthRequest
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
