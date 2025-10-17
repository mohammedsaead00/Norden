using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Models;
using ClothingStoreAPI.Repositories;
using ClothingStoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreAPI.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ClothingStoreContext _context;

    public OrderService(IOrderRepository orderRepository, ClothingStoreContext context)
    {
        _orderRepository = orderRepository;
        _context = context;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order != null ? MapToDto(order) : null;
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto createDto)
    {
        // Calculate total amount
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in createDto.OrderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            var inventory = await _context.Inventories.FindAsync(item.InventoryId);

            if (product == null || inventory == null || inventory.QuantityInStock < item.Quantity)
            {
                return null; // Invalid product or insufficient stock
            }

            var subtotal = product.BasePrice * item.Quantity;
            totalAmount += subtotal;

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                InventoryId = item.InventoryId,
                Quantity = item.Quantity,
                UnitPrice = product.BasePrice,
                Subtotal = subtotal
            });

            // Update inventory
            inventory.QuantityInStock -= item.Quantity;
        }

        var order = new Order
        {
            UserId = createDto.UserId,
            TotalAmount = totalAmount,
            ShippingAddressId = createDto.ShippingAddressId,
            BillingAddressId = createDto.BillingAddressId,
            OrderStatus = "pending",
            OrderItems = orderItems
        };

        var createdOrder = await _orderRepository.AddAsync(order);
        var orderWithDetails = await _orderRepository.GetByIdAsync(createdOrder.OrderId);
        
        return orderWithDetails != null ? MapToDto(orderWithDetails) : null;
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderDto updateDto)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(updateDto.OrderStatus))
            order.OrderStatus = updateDto.OrderStatus;
        
        if (updateDto.TrackingNumber != null)
            order.TrackingNumber = updateDto.TrackingNumber;

        order.UpdatedAt = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        var updatedOrder = await _orderRepository.GetByIdAsync(id);
        
        return updatedOrder != null ? MapToDto(updatedOrder) : null;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        if (!await _orderRepository.ExistsAsync(id))
        {
            return false;
        }

        await _orderRepository.DeleteAsync(id);
        return true;
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            CustomerName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : null,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            OrderStatus = order.OrderStatus,
            ShippingAddressId = order.ShippingAddressId,
            BillingAddressId = order.BillingAddressId,
            TrackingNumber = order.TrackingNumber,
            OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
            {
                OrderItemId = oi.OrderItemId,
                OrderId = oi.OrderId,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.ProductName,
                InventoryId = oi.InventoryId,
                Size = oi.Inventory?.Size,
                Color = oi.Inventory?.Color,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Subtotal = oi.Subtotal
            }).ToList(),
            Payment = order.Payment != null ? new PaymentDto
            {
                PaymentId = order.Payment.PaymentId,
                OrderId = order.Payment.OrderId,
                PaymentMethod = order.Payment.PaymentMethod,
                PaymentStatus = order.Payment.PaymentStatus,
                TransactionId = order.Payment.TransactionId,
                Amount = order.Payment.Amount,
                PaymentDate = order.Payment.PaymentDate
            } : null
        };
    }
}

