using BlazorModularMonolith.Api.Modules.Businesses.Domain.Entities;

namespace BlazorModularMonolith.Api.Modules.Businesses.Domain.Repositories;

public interface IBusinessRepository
{
    Task<Business?> GetByIdAsync(Guid id);
    Task<IEnumerable<Business>> GetAllAsync();
    Task<Business?> GetByTaxIdAsync(string taxId);
    Task<IEnumerable<Business>> GetByTypeAsync(BusinessType type);
    Task<Business> CreateAsync(Business business);
    Task<Business?> UpdateAsync(Business business);
    Task<bool> DeleteAsync(Guid id);
}
