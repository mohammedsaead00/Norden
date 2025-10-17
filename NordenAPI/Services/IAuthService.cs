using NordenAPI.DTOs;

namespace NordenAPI.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<GuestLoginResponse> GuestLoginAsync();
    Task<AuthResponse?> GoogleSignInAsync(string idToken);
    Task<bool> ForgotPasswordAsync(string email);
    Task LogoutAsync(Guid userId, string refreshToken);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
}

