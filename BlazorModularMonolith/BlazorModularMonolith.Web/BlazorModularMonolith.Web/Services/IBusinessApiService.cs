using BlazorModularMonolith.Web.Common;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IBusinessApiService
{
    Task<Result<List<BusinessModel>>> GetAllAsync();
    Task<Result<BusinessModel>> GetByIdAsync(Guid id);
    Task<Result<BusinessModel>> GetByTaxIdAsync(string taxId);
    Task<Result<List<BusinessModel>>> GetByTypeAsync(BusinessType type);
    Task<Result<BusinessModel>> CreateAsync(CreateBusinessModel model);
    Task<Result<BusinessModel>> UpdateAsync(Guid id, CreateBusinessModel model);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<BusinessModel>> AddAddressAsync(Guid businessId, Guid addressId);
    Task<Result<BusinessModel>> RemoveAddressAsync(Guid businessId, Guid addressId);
}
