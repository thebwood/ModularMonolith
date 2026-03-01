using System.Net.Http.Json;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public class AddressApiService : IAddressApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AddressApiService> _logger;
    private const string BaseEndpoint = "api/v1/addresses";

    public AddressApiService(HttpClient httpClient, ILogger<AddressApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<AddressModel>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AddressModel>>(BaseEndpoint);
            return response ?? new List<AddressModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all addresses");
            throw;
        }
    }

    public async Task<AddressModel?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AddressModel>($"{BaseEndpoint}/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching address {AddressId}", id);
            throw;
        }
    }

    public async Task<List<AddressModel>> GetByOwnerIdAsync(Guid ownerId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AddressModel>>($"{BaseEndpoint}/owner/{ownerId}");
            return response ?? new List<AddressModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching addresses for owner {OwnerId}", ownerId);
            throw;
        }
    }

    public async Task<List<AddressModel>> GetByTypeAsync(AddressType type)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AddressModel>>($"{BaseEndpoint}/type/{type}");
            return response ?? new List<AddressModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching addresses of type {Type}", type);
            throw;
        }
    }

    public async Task<AddressModel> CreateAsync(CreateAddressModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, model);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AddressModel>() 
                ?? throw new InvalidOperationException("Failed to deserialize created address");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating address");
            throw;
        }
    }

    public async Task<AddressModel?> UpdateAsync(Guid id, CreateAddressModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}", model);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AddressModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address {AddressId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address {AddressId}", id);
            throw;
        }
    }
}
