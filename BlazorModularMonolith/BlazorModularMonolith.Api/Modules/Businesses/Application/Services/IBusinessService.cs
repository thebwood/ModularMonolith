using BlazorModularMonolith.Api.Modules.Businesses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Businesses.Domain.Entities;

namespace BlazorModularMonolith.Api.Modules.Businesses.Application.Services;

public interface IBusinessService
{
    Task<BusinessDto?> GetBusinessAsync(Guid id);
    Task<IEnumerable<BusinessDto>> GetAllBusinessesAsync();
    Task<BusinessDto?> GetBusinessByTaxIdAsync(string taxId);
    Task<IEnumerable<BusinessDto>> GetBusinessesByTypeAsync(BusinessType type);
    Task<BusinessDto> CreateBusinessAsync(CreateBusinessRequest request);
    Task<BusinessDto?> UpdateBusinessAsync(Guid id, UpdateBusinessRequest request);
    Task<bool> DeleteBusinessAsync(Guid id);
    Task<BusinessDto?> AddAddressToBusinessAsync(Guid businessId, Guid addressId);
    Task<BusinessDto?> RemoveAddressFromBusinessAsync(Guid businessId, Guid addressId);
}
