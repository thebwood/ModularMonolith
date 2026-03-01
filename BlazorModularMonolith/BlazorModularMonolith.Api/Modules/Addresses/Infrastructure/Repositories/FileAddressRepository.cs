using System.Text.Json;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Entities;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Repositories;

namespace BlazorModularMonolith.Api.Modules.Addresses.Infrastructure.Repositories;

public class FileAddressRepository : IAddressRepository
{
    private readonly string _filePath;
    private readonly ILogger<FileAddressRepository> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public FileAddressRepository(IConfiguration configuration, ILogger<FileAddressRepository> logger)
    {
        _logger = logger;
        var dataDirectory = configuration["Storage:DataDirectory"] ?? "Data";
        
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }
        
        _filePath = Path.Combine(dataDirectory, "addresses.json");
        
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<Address?> GetByIdAsync(Guid id)
    {
        var addresses = await ReadAllAsync();
        return addresses.FirstOrDefault(a => a.Id == id);
    }

    public async Task<IEnumerable<Address>> GetAllAsync()
    {
        return await ReadAllAsync();
    }

    public async Task<IEnumerable<Address>> GetByOwnerIdAsync(Guid ownerId)
    {
        var addresses = await ReadAllAsync();
        return addresses.Where(a => a.OwnerId == ownerId);
    }

    public async Task<IEnumerable<Address>> GetByTypeAsync(AddressType type)
    {
        var addresses = await ReadAllAsync();
        return addresses.Where(a => a.Type == type);
    }

    public async Task<Address> CreateAsync(Address address)
    {
        await _semaphore.WaitAsync();
        try
        {
            var addresses = (await ReadAllAsync()).ToList();
            addresses.Add(address);
            await WriteAllAsync(addresses);
            _logger.LogInformation("Created address with ID: {AddressId}", address.Id);
            return address;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Address?> UpdateAsync(Address address)
    {
        await _semaphore.WaitAsync();
        try
        {
            var addresses = (await ReadAllAsync()).ToList();
            var index = addresses.FindIndex(a => a.Id == address.Id);
            
            if (index == -1)
            {
                return null;
            }

            addresses[index] = address;
            await WriteAllAsync(addresses);
            _logger.LogInformation("Updated address with ID: {AddressId}", address.Id);
            return address;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var addresses = (await ReadAllAsync()).ToList();
            var removed = addresses.RemoveAll(a => a.Id == id);
            
            if (removed == 0)
            {
                return false;
            }

            await WriteAllAsync(addresses);
            _logger.LogInformation("Deleted address with ID: {AddressId}", id);
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<List<Address>> ReadAllAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<Address>>(json) ?? new List<Address>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading addresses from file");
            return new List<Address>();
        }
    }

    private async Task WriteAllAsync(List<Address> addresses)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(addresses, options);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing addresses to file");
            throw;
        }
    }
}
