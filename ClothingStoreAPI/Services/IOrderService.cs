using ClothingStoreAPI.DTOs;

namespace ClothingStoreAPI.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
    Task<OrderDto?> CreateOrderAsync(CreateOrderDto createDto);
    Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderDto updateDto);
    Task<bool> DeleteOrderAsync(int id);
}

