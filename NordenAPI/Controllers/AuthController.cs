using Microsoft.AspNetCore.Mvc;
using NordenAPI.DTOs;
using NordenAPI.Services;

namespace NordenAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        
        if (result == null)
        {
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse(
                "DUPLICATE_EMAIL",
                "Email already exists"
            ));
        }

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        
        if (result == null)
        {
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse(
                "INVALID_CREDENTIALS",
                "Invalid email or password"
            ));
        }

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Guest login
    /// </summary>
    [HttpPost("guest")]
    public async Task<ActionResult<ApiResponse<GuestLoginResponse>>> GuestLogin()
    {
        var result = await _authService.GuestLoginAsync();
        return Ok(ApiResponse<GuestLoginResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Google Sign-In
    /// </summary>
    [HttpPost("google")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> GoogleSignIn([FromBody] GoogleSignInRequest request)
    {
        var result = await _authService.GoogleSignInAsync(request.IdToken);
        
        if (result == null)
        {
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse(
                "INVALID_TOKEN",
                "Invalid Google ID token"
            ));
        }

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Forgot password
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request.Email);
        
        // Always return success to prevent email enumeration
        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Password reset email sent" }
        ));
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> Logout()
    {
        // Get user ID from token
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized(ApiResponse<MessageResponse>.ErrorResponse(
                "UNAUTHORIZED",
                "Invalid token"
            ));
        }

        // Get refresh token from header or body
        var refreshToken = Request.Headers["X-Refresh-Token"].FirstOrDefault();
        if (refreshToken != null)
        {
            await _authService.LogoutAsync(Guid.Parse(userIdClaim), refreshToken);
        }

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Logged out successfully" }
        ));
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        
        if (result == null)
        {
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse(
                "INVALID_TOKEN",
                "Invalid or expired refresh token"
            ));
        }

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
    }
}

