using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NordenAPI.Data;
using NordenAPI.DTOs;
using NordenAPI.Models;
using System.Security.Claims;
using System.Text.Json;

namespace NordenAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly NordenDbContext _context;

    public OrdersController(NordenDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Create order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateOrderResponse>>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(request.ShippingAddressId, out var addressId))
        {
            return BadRequest(ApiResponse<CreateOrderResponse>.ErrorResponse(
                "INVALID_ID", "Invalid address ID"
            ));
        }

        // Verify address belongs to user
        var addressExists = await _context.Addresses
            .AnyAsync(a => a.Id == addressId && a.UserId == userId);

        if (!addressExists)
        {
            return BadRequest(ApiResponse<CreateOrderResponse>.ErrorResponse(
                "INVALID_ADDRESS", "Address not found"
            ));
        }

        // Generate order number
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";

        var order = new Order
        {
            UserId = userId,
            OrderNumber = orderNumber,
            Status = OrderStatus.Pending,
            Subtotal = request.Subtotal,
            Tax = request.Tax,
            Shipping = request.Shipping,
            Total = request.Total,
            PaymentMethod = request.PaymentMethod.ToLower() == "card" 
                ? PaymentMethodType.Card 
                : PaymentMethodType.CashOnDelivery,
            ShippingAddressId = addressId
        };

        _context.Orders.Add(order);

        // Add order items and update stock
        foreach (var item in request.Items)
        {
            if (!Guid.TryParse(item.ProductId, out var productId))
                continue;

            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.Stock < item.Quantity)
            {
                return BadRequest(ApiResponse<CreateOrderResponse>.ErrorResponse(
                    "INSUFFICIENT_STOCK", $"Insufficient stock for product {item.ProductId}"
                ));
            }

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductName = product.Name,
                ProductImage = JsonSerializer.Deserialize<List<string>>(product.Images)?.FirstOrDefault() ?? "",
                Quantity = item.Quantity,
                SelectedColor = item.SelectedColor,
                SelectedSize = item.SelectedSize,
                Price = item.Price,
                Subtotal = item.Price * item.Quantity
            };

            _context.OrderItems.Add(orderItem);

            // Update stock
            product.Stock -= item.Quantity;
        }

        await _context.SaveChangesAsync();

        // Clear cart after order
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
        if (cart != null)
        {
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
        }

        var response = new CreateOrderResponse
        {
            OrderId = order.Id.ToString(),
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt
        };

        return Ok(ApiResponse<CreateOrderResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// Get user orders
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<OrderListResponse>>> GetOrders(
        [FromQuery] string? status,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var userId = GetUserId();

        var query = _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.ShippingAddress)
            .Where(o => o.UserId == userId);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
        {
            query = query.Where(o => o.Status == orderStatus);
        }

        var total = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();

        var response = new OrderListResponse
        {
            Orders = orderDtos,
            Total = total,
            Limit = limit,
            Offset = offset
        };

        return Ok(ApiResponse<OrderListResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(string id)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(id, out var orderId))
        {
            return BadRequest(ApiResponse<OrderDto>.ErrorResponse(
                "INVALID_ID", "Invalid order ID"
            ));
        }

        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
        {
            return NotFound(ApiResponse<OrderDto>.ErrorResponse(
                "NOT_FOUND", "Order not found"
            ));
        }

        return Ok(ApiResponse<OrderDto>.SuccessResponse(MapToOrderDto(order)));
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> CancelOrder(string id)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(id, out var orderId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid order ID"
            ));
        }

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
        {
            return NotFound(ApiResponse<MessageResponse>.ErrorResponse(
                "NOT_FOUND", "Order not found"
            ));
        }

        if (order.Status != OrderStatus.Pending)
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "CANNOT_CANCEL", "Only pending orders can be cancelled"
            ));
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        // Restore stock
        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.Stock += item.Quantity;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Order cancelled successfully" }
        ));
    }

    private OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id.ToString(),
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            Items = order.Items.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId.ToString(),
                ProductName = oi.ProductName,
                ProductImage = oi.ProductImage,
                Quantity = oi.Quantity,
                SelectedColor = oi.SelectedColor,
                SelectedSize = oi.SelectedSize,
                Price = oi.Price,
                Subtotal = oi.Subtotal
            }).ToList(),
            Subtotal = order.Subtotal,
            Tax = order.Tax,
            Shipping = order.Shipping,
            Total = order.Total,
            PaymentMethod = order.PaymentMethod.ToString(),
            ShippingAddress = order.ShippingAddress != null ? new AddressDto
            {
                Id = order.ShippingAddress.Id.ToString(),
                Label = order.ShippingAddress.Label,
                Name = order.ShippingAddress.Name,
                Phone = order.ShippingAddress.Phone,
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                Country = order.ShippingAddress.Country,
                IsDefault = order.ShippingAddress.IsDefault,
                CreatedAt = order.ShippingAddress.CreatedAt
            } : null,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}

