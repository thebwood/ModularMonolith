using System.Net.Http.Json;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public class BusinessApiService : IBusinessApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BusinessApiService> _logger;
    private const string BaseEndpoint = "api/v1/businesses";

    public BusinessApiService(HttpClient httpClient, ILogger<BusinessApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<BusinessModel>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BusinessModel>>(BaseEndpoint);
            return response ?? new List<BusinessModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all businesses");
            throw;
        }
    }

    public async Task<BusinessModel?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BusinessModel>($"{BaseEndpoint}/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching business {BusinessId}", id);
            throw;
        }
    }

    public async Task<BusinessModel?> GetByTaxIdAsync(string taxId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BusinessModel>($"{BaseEndpoint}/taxid/{taxId}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching business by tax ID {TaxId}", taxId);
            throw;
        }
    }

    public async Task<List<BusinessModel>> GetByTypeAsync(BusinessType type)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BusinessModel>>($"{BaseEndpoint}/type/{type}");
            return response ?? new List<BusinessModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching businesses of type {Type}", type);
            throw;
        }
    }

    public async Task<BusinessModel> CreateAsync(CreateBusinessModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, model);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BusinessModel>() 
                ?? throw new InvalidOperationException("Failed to deserialize created business");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating business");
            throw;
        }
    }

    public async Task<BusinessModel?> UpdateAsync(Guid id, CreateBusinessModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}", model);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BusinessModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating business {BusinessId}", id);
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
            _logger.LogError(ex, "Error deleting business {BusinessId}", id);
            throw;
        }
    }

    public async Task<BusinessModel?> AddAddressAsync(Guid businessId, Guid addressId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"{BaseEndpoint}/{businessId}/addresses/{addressId}", null);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BusinessModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address {AddressId} to business {BusinessId}", addressId, businessId);
            throw;
        }
    }

    public async Task<BusinessModel?> RemoveAddressAsync(Guid businessId, Guid addressId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{businessId}/addresses/{addressId}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BusinessModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing address {AddressId} from business {BusinessId}", addressId, businessId);
            throw;
        }
    }
}
