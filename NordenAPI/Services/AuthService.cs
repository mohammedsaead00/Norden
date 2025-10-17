using Microsoft.EntityFrameworkCore;
using NordenAPI.Data;
using NordenAPI.DTOs;
using NordenAPI.Models;

namespace NordenAPI.Services;

public class AuthService : IAuthService
{
    private readonly NordenDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(NordenDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if email exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return null;
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            DisplayName = request.DisplayName,
            PhoneNumber = request.PhoneNumber,
            IsGuest = false,
            IsAdmin = false
        };

        _context.Users.Add(user);
        
        // Create empty cart for user
        var cart = new Cart { UserId = user.Id };
        _context.Carts.Add(cart);
        
        await _context.SaveChangesAsync();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenString = _jwtService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            DisplayName = user.DisplayName,
            IsAdmin = user.IsAdmin,
            IsGuest = false,
            Token = accessToken,
            RefreshToken = refreshTokenString
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenString = _jwtService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            DisplayName = user.DisplayName,
            IsAdmin = user.IsAdmin,
            IsGuest = false,
            Token = accessToken,
            RefreshToken = refreshTokenString
        };
    }

    public async Task<GuestLoginResponse> GuestLoginAsync()
    {
        var user = new User
        {
            Email = $"guest_{Guid.NewGuid()}@norden.com",
            DisplayName = "Guest User",
            IsGuest = true,
            IsAdmin = false
        };

        _context.Users.Add(user);
        
        // Create empty cart for guest
        var cart = new Cart { UserId = user.Id };
        _context.Carts.Add(cart);
        
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user);

        return new GuestLoginResponse
        {
            UserId = user.Id.ToString(),
            IsGuest = true,
            Token = accessToken
        };
    }

    public async Task<AuthResponse?> GoogleSignInAsync(string idToken)
    {
        // TODO: Verify Google ID token with Google API
        // For now, this is a placeholder
        // In production, use Google.Apis.Auth library to verify the token
        
        return null; // Implement Google token verification
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false;
        }

        // TODO: Send password reset email
        // Generate reset token and send email
        
        return true;
    }

    public async Task LogoutAsync(Guid userId, string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == refreshToken);

        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || !token.IsActive)
        {
            return null;
        }

        // Revoke old token
        token.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(token.User!);
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            UserId = token.UserId,
            Token = newRefreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = token.User!.Id.ToString(),
            Email = token.User.Email,
            DisplayName = token.User.DisplayName,
            IsAdmin = token.User.IsAdmin,
            IsGuest = token.User.IsGuest,
            Token = accessToken,
            RefreshToken = newRefreshTokenString
        };
    }
}

