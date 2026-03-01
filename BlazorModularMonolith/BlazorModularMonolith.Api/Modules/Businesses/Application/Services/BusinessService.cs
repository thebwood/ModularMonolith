using BlazorModularMonolith.Api.Modules.Businesses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Businesses.Domain.Entities;
using BlazorModularMonolith.Api.Modules.Businesses.Domain.Repositories;

namespace BlazorModularMonolith.Api.Modules.Businesses.Application.Services;

public class BusinessService : IBusinessService
{
    private readonly IBusinessRepository _repository;
    private readonly ILogger<BusinessService> _logger;

    public BusinessService(IBusinessRepository repository, ILogger<BusinessService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<BusinessDto?> GetBusinessAsync(Guid id)
    {
        _logger.LogInformation("Retrieving business with ID: {BusinessId}", id);
        var business = await _repository.GetByIdAsync(id);
        return business != null ? MapToDto(business) : null;
    }

    public async Task<IEnumerable<BusinessDto>> GetAllBusinessesAsync()
    {
        _logger.LogInformation("Retrieving all businesses");
        var businesses = await _repository.GetAllAsync();
        return businesses.Select(MapToDto);
    }

    public async Task<BusinessDto?> GetBusinessByTaxIdAsync(string taxId)
    {
        _logger.LogInformation("Retrieving business with Tax ID: {TaxId}", taxId);
        var business = await _repository.GetByTaxIdAsync(taxId);
        return business != null ? MapToDto(business) : null;
    }

    public async Task<IEnumerable<BusinessDto>> GetBusinessesByTypeAsync(BusinessType type)
    {
        _logger.LogInformation("Retrieving businesses of type: {BusinessType}", type);
        var businesses = await _repository.GetByTypeAsync(type);
        return businesses.Select(MapToDto);
    }

    public async Task<BusinessDto> CreateBusinessAsync(CreateBusinessRequest request)
    {
        _logger.LogInformation("Creating new business: {Name}", request.Name);
        
        var business = new Business
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TaxId = request.TaxId,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Website = request.Website,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(business);
        return MapToDto(created);
    }

    public async Task<BusinessDto?> UpdateBusinessAsync(Guid id, UpdateBusinessRequest request)
    {
        _logger.LogInformation("Updating business with ID: {BusinessId}", id);
        
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Business with ID: {BusinessId} not found", id);
            return null;
        }

        existing.Name = request.Name;
        existing.TaxId = request.TaxId;
        existing.Email = request.Email;
        existing.PhoneNumber = request.PhoneNumber;
        existing.Website = request.Website;
        existing.Type = request.Type;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        return updated != null ? MapToDto(updated) : null;
    }

    public async Task<bool> DeleteBusinessAsync(Guid id)
    {
        _logger.LogInformation("Deleting business with ID: {BusinessId}", id);
        return await _repository.DeleteAsync(id);
    }

    public async Task<BusinessDto?> AddAddressToBusinessAsync(Guid businessId, Guid addressId)
    {
        _logger.LogInformation("Adding address {AddressId} to business {BusinessId}", addressId, businessId);
        
        var business = await _repository.GetByIdAsync(businessId);
        if (business == null)
        {
            _logger.LogWarning("Business with ID: {BusinessId} not found", businessId);
            return null;
        }

        if (!business.AddressIds.Contains(addressId))
        {
            business.AddressIds.Add(addressId);
            business.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(business);
        }

        return MapToDto(business);
    }

    public async Task<BusinessDto?> RemoveAddressFromBusinessAsync(Guid businessId, Guid addressId)
    {
        _logger.LogInformation("Removing address {AddressId} from business {BusinessId}", addressId, businessId);
        
        var business = await _repository.GetByIdAsync(businessId);
        if (business == null)
        {
            _logger.LogWarning("Business with ID: {BusinessId} not found", businessId);
            return null;
        }

        business.AddressIds.Remove(addressId);
        business.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(business);

        return MapToDto(business);
    }

    private static BusinessDto MapToDto(Business business) => new(
        business.Id,
        business.Name,
        business.TaxId,
        business.Email,
        business.PhoneNumber,
        business.Website,
        business.Type,
        business.AddressIds,
        business.CreatedAt,
        business.UpdatedAt
    );
}
