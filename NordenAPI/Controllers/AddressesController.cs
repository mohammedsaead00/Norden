using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NordenAPI.Data;
using NordenAPI.DTOs;
using NordenAPI.Models;
using System.Security.Claims;

namespace NordenAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/addresses")]
public class AddressesController : ControllerBase
{
    private readonly NordenDbContext _context;

    public AddressesController(NordenDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Get user addresses
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<AddressListResponse>>> GetAddresses()
    {
        var userId = GetUserId();

        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();

        var addressDtos = addresses.Select(a => new AddressDto
        {
            Id = a.Id.ToString(),
            Label = a.Label,
            Name = a.Name,
            Phone = a.Phone,
            Street = a.Street,
            City = a.City,
            Country = a.Country,
            IsDefault = a.IsDefault,
            CreatedAt = a.CreatedAt
        }).ToList();

        return Ok(ApiResponse<AddressListResponse>.SuccessResponse(
            new AddressListResponse { Addresses = addressDtos }
        ));
    }

    /// <summary>
    /// Add address
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateAddressResponse>>> AddAddress([FromBody] CreateAddressRequest request)
    {
        var userId = GetUserId();

        // If setting as default, unset other default addresses
        if (request.IsDefault)
        {
            var existingDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();
            
            foreach (var addr in existingDefaults)
            {
                addr.IsDefault = false;
            }
        }

        var address = new Address
        {
            UserId = userId,
            Label = request.Label,
            Name = request.Name,
            Phone = request.Phone,
            Street = request.Street,
            City = request.City,
            Country = request.Country,
            IsDefault = request.IsDefault
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<CreateAddressResponse>.SuccessResponse(
            new CreateAddressResponse { AddressId = address.Id.ToString() }
        ));
    }

    /// <summary>
    /// Update address
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> UpdateAddress(
        string id, 
        [FromBody] UpdateAddressRequest request)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(id, out var addressId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid address ID"
            ));
        }

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
        {
            return NotFound(ApiResponse<MessageResponse>.ErrorResponse(
                "NOT_FOUND", "Address not found"
            ));
        }

        if (!string.IsNullOrEmpty(request.Label))
            address.Label = request.Label;
        if (!string.IsNullOrEmpty(request.Name))
            address.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Phone))
            address.Phone = request.Phone;
        if (!string.IsNullOrEmpty(request.Street))
            address.Street = request.Street;
        if (!string.IsNullOrEmpty(request.City))
            address.City = request.City;
        if (!string.IsNullOrEmpty(request.Country))
            address.Country = request.Country;
        
        if (request.IsDefault.HasValue && request.IsDefault.Value)
        {
            var existingDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId)
                .ToListAsync();
            
            foreach (var addr in existingDefaults)
            {
                addr.IsDefault = false;
            }
            address.IsDefault = true;
        }

        address.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Address updated successfully" }
        ));
    }

    /// <summary>
    /// Delete address
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> DeleteAddress(string id)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(id, out var addressId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid address ID"
            ));
        }

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
        {
            return NotFound(ApiResponse<MessageResponse>.ErrorResponse(
                "NOT_FOUND", "Address not found"
            ));
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Address deleted successfully" }
        ));
    }

    /// <summary>
    /// Set default address
    /// </summary>
    [HttpPost("{id}/set-default")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> SetDefaultAddress(string id)
    {
        var userId = GetUserId();

        if (!Guid.TryParse(id, out var addressId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse(
                "INVALID_ID", "Invalid address ID"
            ));
        }

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
        {
            return NotFound(ApiResponse<MessageResponse>.ErrorResponse(
                "NOT_FOUND", "Address not found"
            ));
        }

        // Unset other defaults
        var existingDefaults = await _context.Addresses
            .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId)
            .ToListAsync();
        
        foreach (var addr in existingDefaults)
        {
            addr.IsDefault = false;
        }

        address.IsDefault = true;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<MessageResponse>.SuccessResponse(
            new MessageResponse { Message = "Default address updated" }
        ));
    }
}

