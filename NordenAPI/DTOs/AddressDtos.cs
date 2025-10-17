using System.ComponentModel.DataAnnotations;

namespace NordenAPI.DTOs;

public class AddressDto
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAddressRequest
{
    [Required]
    public string Label { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Street { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}

public class UpdateAddressRequest
{
    public string? Label { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool? IsDefault { get; set; }
}

public class AddressListResponse
{
    public List<AddressDto> Addresses { get; set; } = new();
}

public class CreateAddressResponse
{
    public string AddressId { get; set; } = string.Empty;
}

