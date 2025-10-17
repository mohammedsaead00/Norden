using Microsoft.EntityFrameworkCore;
using ClothingStoreAPI.Data;
using ClothingStoreAPI.Models;

namespace ClothingStoreAPI.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ClothingStoreContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet.Include(u => u.Role).ToListAsync();
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}

