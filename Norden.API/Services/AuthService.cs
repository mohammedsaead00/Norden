using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Norden.API.Data;
using Norden.API.DTOs.Auth;
using Norden.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace Norden.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("User with this email already exists");
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create user
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    Name = request.Name ?? "",
                    Phone = request.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Cart = new Cart() // Create an empty cart for new users
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate tokens
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Save refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new AuthResponse
                {
                    UserId = user.Id.ToString(),
                    Email = user.Email,
                    Name = user.Name,
                    AvatarUrl = user.AvatarUrl,
                    IsAdmin = user.IsAdmin,
                    Token = token,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                throw;
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                // Generate tokens
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Update last login and tokens
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new AuthResponse
                {
                    UserId = user.Id.ToString(),
                    Email = user.Email,
                    Name = user.Name,
                    AvatarUrl = user.AvatarUrl,
                    IsAdmin = user.IsAdmin,
                    Token = token,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                throw;
            }
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                // Validate the refresh token against the database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && u.IsActive);
                
                if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Invalid or expired refresh token");
                }

                // Generate new tokens
                var token = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();
                
                // Update the user's refresh token (rotation policy)
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();

                return new TokenResponse
                {
                    Token = token,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                throw;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);
                if (user == null)
                {
                    // Don't reveal if user exists or not
                    return true;
                }

                // In a real implementation, you would send an email with reset link
                // For now, we'll just log it
                _logger.LogInformation($"Password reset requested for user: {request.Email}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password");
                throw;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured"));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetUserIdFromTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "userId");
                return userIdClaim?.Value ?? throw new UnauthorizedAccessException("Invalid token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user ID from token");
                throw new UnauthorizedAccessException("Invalid token");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim("isAdmin", user.IsAdmin.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
