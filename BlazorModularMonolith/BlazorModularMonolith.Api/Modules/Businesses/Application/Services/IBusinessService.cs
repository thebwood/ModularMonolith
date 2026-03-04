using BlazorModularMonolith.Api.Modules.Businesses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Businesses.Domain.Entities;
using BlazorModularMonolith.Api.Shared.Common;

namespace BlazorModularMonolith.Api.Modules.Businesses.Application.Services;

public interface IBusinessService
{
    Task<Result<BusinessDto>> GetBusinessAsync(Guid id);
    Task<Result<IEnumerable<BusinessDto>>> GetAllBusinessesAsync();
    Task<Result<BusinessDto>> GetBusinessByTaxIdAsync(string taxId);
    Task<Result<IEnumerable<BusinessDto>>> GetBusinessesByTypeAsync(BusinessType type);
    Task<Result<BusinessDto>> CreateBusinessAsync(CreateBusinessRequest request);
    Task<Result<BusinessDto>> UpdateBusinessAsync(Guid id, UpdateBusinessRequest request);
    Task<Result> DeleteBusinessAsync(Guid id);
    Task<Result<BusinessDto>> AddAddressToBusinessAsync(Guid businessId, Guid addressId);
    Task<Result<BusinessDto>> RemoveAddressFromBusinessAsync(Guid businessId, Guid addressId);
}
