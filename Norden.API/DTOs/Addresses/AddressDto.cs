using Norden.API.DTOs.Common;

namespace Norden.API.DTOs.Addresses
{
    public class AddressDto
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAddressRequest
    {
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }

    public class UpdateAddressRequest
    {
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }
}
