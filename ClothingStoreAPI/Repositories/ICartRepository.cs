using ClothingStoreAPI.Models;

namespace ClothingStoreAPI.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<IEnumerable<Cart>> GetByUserIdAsync(int userId);
    Task ClearUserCartAsync(int userId);
}

