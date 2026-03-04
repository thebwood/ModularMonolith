using BlazorModularMonolith.Web.Common;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IAddressApiService
{
    Task<Result<List<AddressModel>>> GetAllAsync();
    Task<Result<AddressModel>> GetByIdAsync(Guid id);
    Task<Result<List<AddressModel>>> GetByOwnerIdAsync(Guid ownerId);
    Task<Result<List<AddressModel>>> GetByTypeAsync(AddressType type);
    Task<Result<AddressModel>> CreateAsync(CreateAddressModel model);
    Task<Result<AddressModel>> UpdateAsync(Guid id, CreateAddressModel model);
    Task<Result> DeleteAsync(Guid id);
}
