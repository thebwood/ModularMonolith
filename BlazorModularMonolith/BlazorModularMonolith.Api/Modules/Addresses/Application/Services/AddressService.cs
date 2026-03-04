using BlazorModularMonolith.Api.Modules.Addresses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Entities;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Repositories;
using BlazorModularMonolith.Api.Shared.Common;

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

    public async Task<Result<AddressDto>> GetAddressAsync(Guid id)
    {
        _logger.LogInformation("Retrieving address with ID: {AddressId}", id);
        var address = await _repository.GetByIdAsync(id);
        return address is not null
            ? Result<AddressDto>.Success(MapToDto(address))
            : Result<AddressDto>.Failure($"Address with ID '{id}' not found.");
    }

    public async Task<Result<IEnumerable<AddressDto>>> GetAllAddressesAsync()
    {
        _logger.LogInformation("Retrieving all addresses");
        var addresses = await _repository.GetAllAsync();
        return Result<IEnumerable<AddressDto>>.Success(addresses.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<AddressDto>>> GetAddressesByOwnerAsync(Guid ownerId)
    {
        _logger.LogInformation("Retrieving addresses for owner: {OwnerId}", ownerId);
        var addresses = await _repository.GetByOwnerIdAsync(ownerId);
        return Result<IEnumerable<AddressDto>>.Success(addresses.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<AddressDto>>> GetAddressesByTypeAsync(AddressType type)
    {
        _logger.LogInformation("Retrieving addresses of type: {AddressType}", type);
        var addresses = await _repository.GetByTypeAsync(type);
        return Result<IEnumerable<AddressDto>>.Success(addresses.Select(MapToDto));
    }

    public async Task<Result<AddressDto>> CreateAddressAsync(CreateAddressRequest request)
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
        return Result<AddressDto>.Success(MapToDto(created));
    }

    public async Task<Result<AddressDto>> UpdateAddressAsync(Guid id, UpdateAddressRequest request)
    {
        _logger.LogInformation("Updating address with ID: {AddressId}", id);

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Address with ID: {AddressId} not found", id);
            return Result<AddressDto>.Failure($"Address with ID '{id}' not found.");
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
        return updated is not null
            ? Result<AddressDto>.Success(MapToDto(updated))
            : Result<AddressDto>.Failure($"Address with ID '{id}' not found.");
    }

    public async Task<Result> DeleteAddressAsync(Guid id)
    {
        _logger.LogInformation("Deleting address with ID: {AddressId}", id);
        var deleted = await _repository.DeleteAsync(id);
        return deleted
            ? Result.Success()
            : Result.Failure($"Address with ID '{id}' not found.");
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
