using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IPersonApiService
{
    Task<List<PersonModel>> GetAllAsync();
    Task<PersonModel?> GetByIdAsync(Guid id);
    Task<PersonModel?> GetByEmailAsync(string email);
    Task<PersonModel> CreateAsync(CreatePersonModel model);
    Task<PersonModel?> UpdateAsync(Guid id, CreatePersonModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<PersonModel?> AddAddressAsync(Guid personId, Guid addressId);
    Task<PersonModel?> RemoveAddressAsync(Guid personId, Guid addressId);
}
