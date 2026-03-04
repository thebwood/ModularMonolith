using BlazorModularMonolith.Web.Common;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IPersonApiService
{
    Task<Result<List<PersonModel>>> GetAllAsync();
    Task<Result<PersonModel>> GetByIdAsync(Guid id);
    Task<Result<PersonModel>> GetByEmailAsync(string email);
    Task<Result<PersonModel>> CreateAsync(CreatePersonModel model);
    Task<Result<PersonModel>> UpdateAsync(Guid id, CreatePersonModel model);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<PersonModel>> AddAddressAsync(Guid personId, Guid addressId);
    Task<Result<PersonModel>> RemoveAddressAsync(Guid personId, Guid addressId);
}
