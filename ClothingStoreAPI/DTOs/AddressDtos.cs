using System.ComponentModel.DataAnnotations;

namespace ClothingStoreAPI.DTOs;

public class AddressDto
{
    public int AddressId { get; set; }
    public int UserId { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string? Apartment { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAddressDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string AddressType { get; set; } = string.Empty;

    [Required]
    public string StreetAddress { get; set; } = string.Empty;

    public string? Apartment { get; set; }

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string State { get; set; } = string.Empty;

    [Required]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = "USA";

    public bool IsDefault { get; set; }
}

public class UpdateAddressDto
{
    public string? AddressType { get; set; }
    public string? StreetAddress { get; set; }
    public string? Apartment { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool? IsDefault { get; set; }
}

