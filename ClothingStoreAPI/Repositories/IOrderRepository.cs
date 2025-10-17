using ClothingStoreAPI.Models;

namespace ClothingStoreAPI.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<Order?> GetOrderWithDetailsAsync(int orderId);
}

