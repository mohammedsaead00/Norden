using Microsoft.AspNetCore.Mvc;
using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Models;
using ClothingStoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly ClothingStoreContext _context;

    public AddressesController(ClothingStoreContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all addresses
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll()
    {
        var addresses = await _context.Addresses.ToListAsync();

        var addressDtos = addresses.Select(a => new AddressDto
        {
            AddressId = a.AddressId,
            UserId = a.UserId,
            AddressType = a.AddressType,
            StreetAddress = a.StreetAddress,
            Apartment = a.Apartment,
            City = a.City,
            State = a.State,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsDefault = a.IsDefault,
            CreatedAt = a.CreatedAt
        });

        return Ok(addressDtos);
    }

    /// <summary>
    /// Get address by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AddressDto>> GetById(int id)
    {
        var address = await _context.Addresses.FindAsync(id);
        
        if (address == null)
        {
            return NotFound(new { message = "Address not found" });
        }

        var addressDto = new AddressDto
        {
            AddressId = address.AddressId,
            UserId = address.UserId,
            AddressType = address.AddressType,
            StreetAddress = address.StreetAddress,
            Apartment = address.Apartment,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt
        };

        return Ok(addressDto);
    }

    /// <summary>
    /// Get addresses by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetByUserId(int userId)
    {
        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync();

        var addressDtos = addresses.Select(a => new AddressDto
        {
            AddressId = a.AddressId,
            UserId = a.UserId,
            AddressType = a.AddressType,
            StreetAddress = a.StreetAddress,
            Apartment = a.Apartment,
            City = a.City,
            State = a.State,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsDefault = a.IsDefault,
            CreatedAt = a.CreatedAt
        });

        return Ok(addressDtos);
    }

    /// <summary>
    /// Create new address
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AddressDto>> Create([FromBody] CreateAddressDto createDto)
    {
        var address = new Address
        {
            UserId = createDto.UserId,
            AddressType = createDto.AddressType,
            StreetAddress = createDto.StreetAddress,
            Apartment = createDto.Apartment,
            City = createDto.City,
            State = createDto.State,
            PostalCode = createDto.PostalCode,
            Country = createDto.Country,
            IsDefault = createDto.IsDefault
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        var addressDto = new AddressDto
        {
            AddressId = address.AddressId,
            UserId = address.UserId,
            AddressType = address.AddressType,
            StreetAddress = address.StreetAddress,
            Apartment = address.Apartment,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = address.AddressId }, addressDto);
    }

    /// <summary>
    /// Update address
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AddressDto>> Update(int id, [FromBody] UpdateAddressDto updateDto)
    {
        var address = await _context.Addresses.FindAsync(id);
        
        if (address == null)
        {
            return NotFound(new { message = "Address not found" });
        }

        if (!string.IsNullOrEmpty(updateDto.AddressType))
            address.AddressType = updateDto.AddressType;
        
        if (!string.IsNullOrEmpty(updateDto.StreetAddress))
            address.StreetAddress = updateDto.StreetAddress;
        
        if (updateDto.Apartment != null)
            address.Apartment = updateDto.Apartment;
        
        if (!string.IsNullOrEmpty(updateDto.City))
            address.City = updateDto.City;
        
        if (!string.IsNullOrEmpty(updateDto.State))
            address.State = updateDto.State;
        
        if (!string.IsNullOrEmpty(updateDto.PostalCode))
            address.PostalCode = updateDto.PostalCode;
        
        if (!string.IsNullOrEmpty(updateDto.Country))
            address.Country = updateDto.Country;
        
        if (updateDto.IsDefault.HasValue)
            address.IsDefault = updateDto.IsDefault.Value;

        await _context.SaveChangesAsync();

        var addressDto = new AddressDto
        {
            AddressId = address.AddressId,
            UserId = address.UserId,
            AddressType = address.AddressType,
            StreetAddress = address.StreetAddress,
            Apartment = address.Apartment,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt
        };

        return Ok(addressDto);
    }

    /// <summary>
    /// Delete address
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var address = await _context.Addresses.FindAsync(id);
        
        if (address == null)
        {
            return NotFound(new { message = "Address not found" });
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

