using Microsoft.AspNetCore.Mvc;
using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly ClothingStoreContext _context;

    public OrderItemsController(ClothingStoreContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get order items by order ID
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(int orderId)
    {
        var orderItems = await _context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Inventory)
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        var orderItemDtos = orderItems.Select(oi => new OrderItemDto
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
        });

        return Ok(orderItemDtos);
    }

    /// <summary>
    /// Get order item by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItemDto>> GetById(int id)
    {
        var orderItem = await _context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Inventory)
            .FirstOrDefaultAsync(oi => oi.OrderItemId == id);
        
        if (orderItem == null)
        {
            return NotFound(new { message = "Order item not found" });
        }

        var orderItemDto = new OrderItemDto
        {
            OrderItemId = orderItem.OrderItemId,
            OrderId = orderItem.OrderId,
            ProductId = orderItem.ProductId,
            ProductName = orderItem.Product?.ProductName,
            InventoryId = orderItem.InventoryId,
            Size = orderItem.Inventory?.Size,
            Color = orderItem.Inventory?.Color,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            Subtotal = orderItem.Subtotal
        };

        return Ok(orderItemDto);
    }
}

