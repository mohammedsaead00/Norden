using ClothingStoreAPI.DTOs;

namespace ClothingStoreAPI.Services;

public interface IUserService
{
    Task<UserDto?> RegisterAsync(RegisterDto registerDto);
    Task<UserDto?> LoginAsync(LoginDto loginDto);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateDto);
    Task<bool> DeleteUserAsync(int id);
}

