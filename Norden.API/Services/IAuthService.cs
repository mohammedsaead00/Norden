using Norden.API.DTOs.Auth;

namespace Norden.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<bool> ValidateTokenAsync(string token);
        Task<string> GetUserIdFromTokenAsync(string token);
    }
}
