using BlazorModularMonolith.Api.Modules.Businesses.Domain.Entities;

namespace BlazorModularMonolith.Api.Modules.Businesses.Application.DTOs;

public record BusinessDto(
    Guid Id,
    string Name,
    string TaxId,
    string Email,
    string PhoneNumber,
    string Website,
    BusinessType Type,
    List<Guid> AddressIds,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateBusinessRequest(
    string Name,
    string TaxId,
    string Email,
    string PhoneNumber,
    string Website,
    BusinessType Type
);

public record UpdateBusinessRequest(
    string Name,
    string TaxId,
    string Email,
    string PhoneNumber,
    string Website,
    BusinessType Type
);
