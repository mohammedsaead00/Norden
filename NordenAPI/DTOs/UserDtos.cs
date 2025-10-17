using System.ComponentModel.DataAnnotations;

namespace NordenAPI.DTOs;

public class UserProfileDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? PhotoURL { get; set; }
    public bool IsGuest { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateProfileRequest
{
    public string? DisplayName { get; set; }
    public string? PhoneNumber { get; set; }
}

public class UploadPhotoResponse
{
    public string PhotoURL { get; set; } = string.Empty;
}

