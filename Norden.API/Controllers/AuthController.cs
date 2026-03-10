using Microsoft.AspNetCore.Mvc;
using Norden.API.DTOs.Auth;
using Norden.API.DTOs.Common;
using Norden.API.Services;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var result = await _authService.RegisterAsync(request);
                return Ok(ApiResponse<AuthResponse>.SuccessResult(result, "User registered successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AuthResponse>.ErrorResult(ex.Message, "USER_EXISTS"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var result = await _authService.LoginAsync(request);
                return Ok(ApiResponse<AuthResponse>.SuccessResult(result, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<AuthResponse>.ErrorResult(ex.Message, "INVALID_CREDENTIALS"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<TokenResponse>>> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<TokenResponse>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var result = await _authService.RefreshTokenAsync(request);
                return Ok(ApiResponse<TokenResponse>.SuccessResult(result, "Token refreshed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, ApiResponse<TokenResponse>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                await _authService.ForgotPasswordAsync(request);
                return Ok(ApiResponse<object>.SuccessResult(null, "Password reset email sent"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }
    }
}
