using System.Text.Json;
using BlazorModularMonolith.Api.Modules.Businesses.Domain.Entities;
using BlazorModularMonolith.Api.Modules.Businesses.Domain.Repositories;

namespace BlazorModularMonolith.Api.Modules.Businesses.Infrastructure.Repositories;

public class FileBusinessRepository : IBusinessRepository
{
    private readonly string _filePath;
    private readonly ILogger<FileBusinessRepository> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public FileBusinessRepository(IConfiguration configuration, ILogger<FileBusinessRepository> logger)
    {
        _logger = logger;
        var dataDirectory = configuration["Storage:DataDirectory"] ?? "Data";
        
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }
        
        _filePath = Path.Combine(dataDirectory, "businesses.json");
        
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<Business?> GetByIdAsync(Guid id)
    {
        var businesses = await ReadAllAsync();
        return businesses.FirstOrDefault(b => b.Id == id);
    }

    public async Task<IEnumerable<Business>> GetAllAsync()
    {
        return await ReadAllAsync();
    }

    public async Task<Business?> GetByTaxIdAsync(string taxId)
    {
        var businesses = await ReadAllAsync();
        return businesses.FirstOrDefault(b => b.TaxId.Equals(taxId, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<Business>> GetByTypeAsync(BusinessType type)
    {
        var businesses = await ReadAllAsync();
        return businesses.Where(b => b.Type == type);
    }

    public async Task<Business> CreateAsync(Business business)
    {
        await _semaphore.WaitAsync();
        try
        {
            var businesses = (await ReadAllAsync()).ToList();
            businesses.Add(business);
            await WriteAllAsync(businesses);
            _logger.LogInformation("Created business with ID: {BusinessId}", business.Id);
            return business;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Business?> UpdateAsync(Business business)
    {
        await _semaphore.WaitAsync();
        try
        {
            var businesses = (await ReadAllAsync()).ToList();
            var index = businesses.FindIndex(b => b.Id == business.Id);
            
            if (index == -1)
            {
                return null;
            }

            businesses[index] = business;
            await WriteAllAsync(businesses);
            _logger.LogInformation("Updated business with ID: {BusinessId}", business.Id);
            return business;
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
            var businesses = (await ReadAllAsync()).ToList();
            var removed = businesses.RemoveAll(b => b.Id == id);
            
            if (removed == 0)
            {
                return false;
            }

            await WriteAllAsync(businesses);
            _logger.LogInformation("Deleted business with ID: {BusinessId}", id);
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<List<Business>> ReadAllAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<Business>>(json) ?? new List<Business>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading businesses from file");
            return new List<Business>();
        }
    }

    private async Task WriteAllAsync(List<Business> businesses)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(businesses, options);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing businesses to file");
            throw;
        }
    }
}
