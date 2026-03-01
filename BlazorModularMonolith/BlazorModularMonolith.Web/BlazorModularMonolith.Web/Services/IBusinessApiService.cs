using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IBusinessApiService
{
    Task<List<BusinessModel>> GetAllAsync();
    Task<BusinessModel?> GetByIdAsync(Guid id);
    Task<BusinessModel?> GetByTaxIdAsync(string taxId);
    Task<List<BusinessModel>> GetByTypeAsync(BusinessType type);
    Task<BusinessModel> CreateAsync(CreateBusinessModel model);
    Task<BusinessModel?> UpdateAsync(Guid id, CreateBusinessModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<BusinessModel?> AddAddressAsync(Guid businessId, Guid addressId);
    Task<BusinessModel?> RemoveAddressAsync(Guid businessId, Guid addressId);
}
