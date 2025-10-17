using Microsoft.AspNetCore.Mvc;
using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Models;
using ClothingStoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ClothingStoreContext _context;

    public PaymentsController(ClothingStoreContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all payments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
    {
        var payments = await _context.Payments
            .Include(p => p.Order)
            .ToListAsync();

        var paymentDtos = payments.Select(p => new PaymentDto
        {
            PaymentId = p.PaymentId,
            OrderId = p.OrderId,
            PaymentMethod = p.PaymentMethod,
            PaymentStatus = p.PaymentStatus,
            TransactionId = p.TransactionId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate
        });

        return Ok(paymentDtos);
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.PaymentId == id);
        
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        var paymentDto = new PaymentDto
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = payment.PaymentStatus,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate
        };

        return Ok(paymentDto);
    }

    /// <summary>
    /// Get payment by order ID
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<PaymentDto>> GetByOrderId(int orderId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
        
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found for this order" });
        }

        var paymentDto = new PaymentDto
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = payment.PaymentStatus,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate
        };

        return Ok(paymentDto);
    }

    /// <summary>
    /// Create new payment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto createDto)
    {
        // Check if order exists
        var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == createDto.OrderId);
        if (!orderExists)
        {
            return BadRequest(new { message = "Order not found" });
        }

        // Check if payment already exists for this order
        var existingPayment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == createDto.OrderId);
        
        if (existingPayment != null)
        {
            return BadRequest(new { message = "Payment already exists for this order" });
        }

        var payment = new Payment
        {
            OrderId = createDto.OrderId,
            PaymentMethod = createDto.PaymentMethod,
            Amount = createDto.Amount,
            TransactionId = createDto.TransactionId,
            PaymentStatus = "pending"
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        var paymentDto = new PaymentDto
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = payment.PaymentStatus,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate
        };

        return CreatedAtAction(nameof(GetById), new { id = payment.PaymentId }, paymentDto);
    }

    /// <summary>
    /// Update payment status
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PaymentDto>> Update(int id, [FromBody] UpdatePaymentDto updateDto)
    {
        var payment = await _context.Payments.FindAsync(id);
        
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        if (!string.IsNullOrEmpty(updateDto.PaymentStatus))
            payment.PaymentStatus = updateDto.PaymentStatus;
        
        if (updateDto.TransactionId != null)
            payment.TransactionId = updateDto.TransactionId;

        await _context.SaveChangesAsync();

        var paymentDto = new PaymentDto
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = payment.PaymentStatus,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate
        };

        return Ok(paymentDto);
    }

    /// <summary>
    /// Delete payment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

