using BlazorModularMonolith.Api.Modules.Addresses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Entities;

namespace BlazorModularMonolith.Api.Modules.Addresses.Application.Services;

public interface IAddressService
{
    Task<AddressDto?> GetAddressAsync(Guid id);
    Task<IEnumerable<AddressDto>> GetAllAddressesAsync();
    Task<IEnumerable<AddressDto>> GetAddressesByOwnerAsync(Guid ownerId);
    Task<IEnumerable<AddressDto>> GetAddressesByTypeAsync(AddressType type);
    Task<AddressDto> CreateAddressAsync(CreateAddressRequest request);
    Task<AddressDto?> UpdateAddressAsync(Guid id, UpdateAddressRequest request);
    Task<bool> DeleteAddressAsync(Guid id);
}
