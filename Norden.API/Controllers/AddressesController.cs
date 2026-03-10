using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Norden.API.Data;
using Norden.API.DTOs.Addresses;
using Norden.API.DTOs.Common;
using Norden.API.Models;
using System.Security.Claims;

namespace Norden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(AppDbContext context, ILogger<AddressesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AddressDto>>>> GetAddresses()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<List<AddressDto>>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var addresses = await _context.Addresses
                    .Where(a => a.UserId == userId.Value)
                    .Select(a => new AddressDto
                    {
                        Id = a.Id.ToString(),
                        Label = a.Label,
                        Name = a.Name,
                        Phone = a.Phone,
                        Street = a.Street,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        PostalCode = a.PostalCode,
                        IsDefault = a.IsDefault,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<AddressDto>>.SuccessResult(addresses));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses");
                return StatusCode(500, ApiResponse<List<AddressDto>>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AddressDto>>> CreateAddress(CreateAddressRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<AddressDto>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<AddressDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                // If this is set as default, unset other defaults
                if (request.IsDefault)
                {
                    var existingDefaults = await _context.Addresses
                        .Where(a => a.UserId == userId.Value && a.IsDefault)
                        .ToListAsync();

                    foreach (var address in existingDefaults)
                    {
                        address.IsDefault = false;
                    }
                }

                var newAddress = new Address
                {
                    UserId = userId.Value,
                    Label = request.Label,
                    Name = request.Name,
                    Phone = request.Phone,
                    Street = request.Street,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    PostalCode = request.PostalCode,
                    IsDefault = request.IsDefault,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();

                var addressDto = new AddressDto
                {
                    Id = newAddress.Id.ToString(),
                    Label = newAddress.Label,
                    Name = newAddress.Name,
                    Phone = newAddress.Phone,
                    Street = newAddress.Street,
                    City = newAddress.City,
                    State = newAddress.State,
                    Country = newAddress.Country,
                    PostalCode = newAddress.PostalCode,
                    IsDefault = newAddress.IsDefault,
                    CreatedAt = newAddress.CreatedAt
                };

                return Ok(ApiResponse<AddressDto>.SuccessResult(addressDto, "Address created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address");
                return StatusCode(500, ApiResponse<AddressDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpPut("{addressId}")]
        public async Task<ActionResult<ApiResponse<AddressDto>>> UpdateAddress(Guid addressId, UpdateAddressRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<AddressDto>.ErrorResult("Invalid input data", "VALIDATION_ERROR", ModelState));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<AddressDto>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId.Value);

                if (address == null)
                {
                    return NotFound(ApiResponse<AddressDto>.ErrorResult("Address not found", "NOT_FOUND"));
                }

                // If this is set as default, unset other defaults
                if (request.IsDefault)
                {
                    var existingDefaults = await _context.Addresses
                        .Where(a => a.UserId == userId.Value && a.IsDefault && a.Id != addressId)
                        .ToListAsync();

                    foreach (var existingAddress in existingDefaults)
                    {
                        existingAddress.IsDefault = false;
                    }
                }

                address.Label = request.Label;
                address.Name = request.Name;
                address.Phone = request.Phone;
                address.Street = request.Street;
                address.City = request.City;
                address.State = request.State;
                address.Country = request.Country;
                address.PostalCode = request.PostalCode;
                address.IsDefault = request.IsDefault;
                address.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var addressDto = new AddressDto
                {
                    Id = address.Id.ToString(),
                    Label = address.Label,
                    Name = address.Name,
                    Phone = address.Phone,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    Country = address.Country,
                    PostalCode = address.PostalCode,
                    IsDefault = address.IsDefault,
                    CreatedAt = address.CreatedAt
                };

                return Ok(ApiResponse<AddressDto>.SuccessResult(addressDto, "Address updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address {AddressId}", addressId);
                return StatusCode(500, ApiResponse<AddressDto>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        [HttpDelete("{addressId}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAddress(Guid addressId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Unauthorized", "UNAUTHORIZED"));
                }

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId.Value);

                if (address == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Address not found", "NOT_FOUND"));
                }

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(null, "Address deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address {AddressId}", addressId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", "INTERNAL_ERROR"));
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
    }
}
