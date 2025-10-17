using Microsoft.AspNetCore.Mvc;
using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Services;

namespace ClothingStoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get all orders
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        
        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        return Ok(order);
    }

    /// <summary>
    /// Get orders by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByUserId(int userId)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(orders);
    }

    /// <summary>
    /// Create new order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto createDto)
    {
        var order = await _orderService.CreateOrderAsync(createDto);
        
        if (order == null)
        {
            return BadRequest(new { message = "Failed to create order. Check product availability and stock." });
        }

        return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
    }

    /// <summary>
    /// Update order status
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] UpdateOrderDto updateDto)
    {
        var order = await _orderService.UpdateOrderStatusAsync(id, updateDto);
        
        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        return Ok(order);
    }

    /// <summary>
    /// Delete order
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = "Order not found" });
        }

        return NoContent();
    }
}

