using System.Net.Http.Json;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public class PersonApiService : IPersonApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PersonApiService> _logger;
    private const string BaseEndpoint = "api/v1/people";

    public PersonApiService(HttpClient httpClient, ILogger<PersonApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<PersonModel>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PersonModel>>(BaseEndpoint);
            return response ?? new List<PersonModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all people");
            throw;
        }
    }

    public async Task<PersonModel?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PersonModel>($"{BaseEndpoint}/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching person {PersonId}", id);
            throw;
        }
    }

    public async Task<PersonModel?> GetByEmailAsync(string email)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PersonModel>($"{BaseEndpoint}/email/{email}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching person by email {Email}", email);
            throw;
        }
    }

    public async Task<PersonModel> CreateAsync(CreatePersonModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, model);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PersonModel>() 
                ?? throw new InvalidOperationException("Failed to deserialize created person");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person");
            throw;
        }
    }

    public async Task<PersonModel?> UpdateAsync(Guid id, CreatePersonModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}", model);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PersonModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person {PersonId}", id);
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
            _logger.LogError(ex, "Error deleting person {PersonId}", id);
            throw;
        }
    }

    public async Task<PersonModel?> AddAddressAsync(Guid personId, Guid addressId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"{BaseEndpoint}/{personId}/addresses/{addressId}", null);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PersonModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address {AddressId} to person {PersonId}", addressId, personId);
            throw;
        }
    }

    public async Task<PersonModel?> RemoveAddressAsync(Guid personId, Guid addressId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{personId}/addresses/{addressId}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PersonModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing address {AddressId} from person {PersonId}", addressId, personId);
            throw;
        }
    }
}
