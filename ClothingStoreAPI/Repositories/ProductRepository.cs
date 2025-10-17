using Microsoft.EntityFrameworkCore;
using ClothingStoreAPI.Data;
using ClothingStoreAPI.Models;

namespace ClothingStoreAPI.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ClothingStoreContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.InventoryItems)
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.InventoryItems)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.InventoryItems)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.InventoryItems)
            .Where(p => p.IsActive && 
                       (p.ProductName.Contains(searchTerm) || 
                        p.Description!.Contains(searchTerm) ||
                        p.Brand!.Contains(searchTerm)))
            .ToListAsync();
    }
}

