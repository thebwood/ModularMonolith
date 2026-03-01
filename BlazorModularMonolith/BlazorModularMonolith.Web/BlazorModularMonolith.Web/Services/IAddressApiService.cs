using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IAddressApiService
{
    Task<List<AddressModel>> GetAllAsync();
    Task<AddressModel?> GetByIdAsync(Guid id);
    Task<List<AddressModel>> GetByOwnerIdAsync(Guid ownerId);
    Task<List<AddressModel>> GetByTypeAsync(AddressType type);
    Task<AddressModel> CreateAsync(CreateAddressModel model);
    Task<AddressModel?> UpdateAsync(Guid id, CreateAddressModel model);
    Task<bool> DeleteAsync(Guid id);
}
