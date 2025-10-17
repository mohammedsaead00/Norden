using Microsoft.EntityFrameworkCore;
using ClothingStoreAPI.Data;
using ClothingStoreAPI.Models;

namespace ClothingStoreAPI.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ClothingStoreContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Cart>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(c => c.Product)
            .Include(c => c.Inventory)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task ClearUserCartAsync(int userId)
    {
        var cartItems = await _dbSet.Where(c => c.UserId == userId).ToListAsync();
        _dbSet.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }
}

