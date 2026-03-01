using BlazorModularMonolith.Api.Modules.Addresses.Domain.Entities;

namespace BlazorModularMonolith.Api.Modules.Addresses.Domain.Repositories;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id);
    Task<IEnumerable<Address>> GetAllAsync();
    Task<IEnumerable<Address>> GetByOwnerIdAsync(Guid ownerId);
    Task<IEnumerable<Address>> GetByTypeAsync(AddressType type);
    Task<Address> CreateAsync(Address address);
    Task<Address?> UpdateAsync(Address address);
    Task<bool> DeleteAsync(Guid id);
}
