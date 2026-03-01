using BlazorModularMonolith.Api.Modules.Addresses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Entities;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Repositories;

namespace BlazorModularMonolith.Api.Modules.Addresses.Application.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _repository;
    private readonly ILogger<AddressService> _logger;

    public AddressService(IAddressRepository repository, ILogger<AddressService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AddressDto?> GetAddressAsync(Guid id)
    {
        _logger.LogInformation("Retrieving address with ID: {AddressId}", id);
        var address = await _repository.GetByIdAsync(id);
        return address != null ? MapToDto(address) : null;
    }

    public async Task<IEnumerable<AddressDto>> GetAllAddressesAsync()
    {
        _logger.LogInformation("Retrieving all addresses");
        var addresses = await _repository.GetAllAsync();
        return addresses.Select(MapToDto);
    }

    public async Task<IEnumerable<AddressDto>> GetAddressesByOwnerAsync(Guid ownerId)
    {
        _logger.LogInformation("Retrieving addresses for owner: {OwnerId}", ownerId);
        var addresses = await _repository.GetByOwnerIdAsync(ownerId);
        return addresses.Select(MapToDto);
    }

    public async Task<IEnumerable<AddressDto>> GetAddressesByTypeAsync(AddressType type)
    {
        _logger.LogInformation("Retrieving addresses of type: {AddressType}", type);
        var addresses = await _repository.GetByTypeAsync(type);
        return addresses.Select(MapToDto);
    }

    public async Task<AddressDto> CreateAddressAsync(CreateAddressRequest request)
    {
        _logger.LogInformation("Creating new address for owner: {OwnerName}", request.OwnerName);
        
        var address = new Address
        {
            Id = Guid.NewGuid(),
            Street = request.Street,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country,
            Type = request.Type,
            OwnerId = request.OwnerId,
            OwnerName = request.OwnerName,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(address);
        return MapToDto(created);
    }

    public async Task<AddressDto?> UpdateAddressAsync(Guid id, UpdateAddressRequest request)
    {
        _logger.LogInformation("Updating address with ID: {AddressId}", id);
        
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Address with ID: {AddressId} not found", id);
            return null;
        }

        existing.Street = request.Street;
        existing.City = request.City;
        existing.State = request.State;
        existing.ZipCode = request.ZipCode;
        existing.Country = request.Country;
        existing.Type = request.Type;
        existing.OwnerId = request.OwnerId;
        existing.OwnerName = request.OwnerName;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        return updated != null ? MapToDto(updated) : null;
    }

    public async Task<bool> DeleteAddressAsync(Guid id)
    {
        _logger.LogInformation("Deleting address with ID: {AddressId}", id);
        return await _repository.DeleteAsync(id);
    }

    private static AddressDto MapToDto(Address address) => new(
        address.Id,
        address.Street,
        address.City,
        address.State,
        address.ZipCode,
        address.Country,
        address.Type,
        address.OwnerId,
        address.OwnerName,
        address.CreatedAt,
        address.UpdatedAt
    );
}
