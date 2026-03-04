using System.Net.Http.Json;
using System.Text.Json;
using BlazorModularMonolith.Web.Common;
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

    public async Task<Result<List<BusinessModel>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BusinessModel>>(BaseEndpoint);
            return Result<List<BusinessModel>>.Success(response ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all businesses");
            return Result<List<BusinessModel>>.Failure($"Failed to load businesses: {ex.Message}");
        }
    }

    public async Task<Result<BusinessModel>> GetByIdAsync(Guid id)
    {
        try
        {
            var business = await _httpClient.GetFromJsonAsync<BusinessModel>($"{BaseEndpoint}/{id}");
            return business is not null
                ? Result<BusinessModel>.Success(business)
                : Result<BusinessModel>.Failure("Business not found.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Result<BusinessModel>.Failure("Business not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching business {BusinessId}", id);
            return Result<BusinessModel>.Failure($"Failed to load business: {ex.Message}");
        }
    }

    public async Task<Result<BusinessModel>> GetByTaxIdAsync(string taxId)
    {
        try
        {
            var business = await _httpClient.GetFromJsonAsync<BusinessModel>($"{BaseEndpoint}/taxid/{taxId}");
            return business is not null
                ? Result<BusinessModel>.Success(business)
                : Result<BusinessModel>.Failure("Business not found.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Result<BusinessModel>.Failure("Business not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching business by tax ID {TaxId}", taxId);
            return Result<BusinessModel>.Failure($"Failed to load business: {ex.Message}");
        }
    }

    public async Task<Result<List<BusinessModel>>> GetByTypeAsync(BusinessType type)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BusinessModel>>($"{BaseEndpoint}/type/{type}");
            return Result<List<BusinessModel>>.Success(response ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching businesses of type {Type}", type);
            return Result<List<BusinessModel>>.Failure($"Failed to load businesses: {ex.Message}");
        }
    }

    public async Task<Result<BusinessModel>> CreateAsync(CreateBusinessModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<BusinessModel>.Failure(error ?? "Failed to create business.");
            }
            var business = await response.Content.ReadFromJsonAsync<BusinessModel>();
            return business is not null
                ? Result<BusinessModel>.Success(business)
                : Result<BusinessModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating business");
            return Result<BusinessModel>.Failure($"Failed to create business: {ex.Message}");
        }
    }

    public async Task<Result<BusinessModel>> UpdateAsync(Guid id, CreateBusinessModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<BusinessModel>.Failure(error ?? "Failed to update business.");
            }
            var business = await response.Content.ReadFromJsonAsync<BusinessModel>();
            return business is not null
                ? Result<BusinessModel>.Success(business)
                : Result<BusinessModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating business {BusinessId}", id);
            return Result<BusinessModel>.Failure($"Failed to update business: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result.Failure(error ?? "Failed to delete business.");
            }
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting business {BusinessId}", id);
            return Result.Failure($"Failed to delete business: {ex.Message}");
        }
    }

    public async Task<Result<BusinessModel>> AddAddressAsync(Guid businessId, Guid addressId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"{BaseEndpoint}/{businessId}/addresses/{addressId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<BusinessModel>.Failure(error ?? "Failed to add address to business.");
            }
            var business = await response.Content.ReadFromJsonAsync<BusinessModel>();
            return business is not null
                ? Result<BusinessModel>.Success(business)
                : Result<BusinessModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address {AddressId} to business {BusinessId}", addressId, businessId);
            return Result<BusinessModel>.Failure($"Failed to add address: {ex.Message}");
        }
    }

    public async Task<Result<BusinessModel>> RemoveAddressAsync(Guid businessId, Guid addressId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{businessId}/addresses/{addressId}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<BusinessModel>.Failure(error ?? "Failed to remove address from business.");
            }
            var business = await response.Content.ReadFromJsonAsync<BusinessModel>();
            return business is not null
                ? Result<BusinessModel>.Success(business)
                : Result<BusinessModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing address {AddressId} from business {BusinessId}", addressId, businessId);
            return Result<BusinessModel>.Failure($"Failed to remove address: {ex.Message}");
        }
    }

    private static async Task<string?> TryReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (body.TryGetProperty("message", out var msg))
                return msg.GetString();
        }
        catch { }
        return null;
    }
}