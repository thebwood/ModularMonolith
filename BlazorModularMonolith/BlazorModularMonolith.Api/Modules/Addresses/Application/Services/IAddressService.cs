using BlazorModularMonolith.Api.Modules.Addresses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Entities;
using BlazorModularMonolith.Api.Shared.Common;

namespace BlazorModularMonolith.Api.Modules.Addresses.Application.Services;

public interface IAddressService
{
    Task<Result<AddressDto>> GetAddressAsync(Guid id);
    Task<Result<IEnumerable<AddressDto>>> GetAllAddressesAsync();
    Task<Result<IEnumerable<AddressDto>>> GetAddressesByOwnerAsync(Guid ownerId);
    Task<Result<IEnumerable<AddressDto>>> GetAddressesByTypeAsync(AddressType type);
    Task<Result<AddressDto>> CreateAddressAsync(CreateAddressRequest request);
    Task<Result<AddressDto>> UpdateAddressAsync(Guid id, UpdateAddressRequest request);
    Task<Result> DeleteAddressAsync(Guid id);
}
