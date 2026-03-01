using BlazorModularMonolith.Api.Modules.Addresses.Domain.Entities;

namespace BlazorModularMonolith.Api.Modules.Addresses.Application.DTOs;

public record AddressDto(
    Guid Id,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    AddressType Type,
    Guid OwnerId,
    string OwnerName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateAddressRequest(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    AddressType Type,
    Guid OwnerId,
    string OwnerName
);

public record UpdateAddressRequest(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    AddressType Type,
    Guid OwnerId,
    string OwnerName
);
