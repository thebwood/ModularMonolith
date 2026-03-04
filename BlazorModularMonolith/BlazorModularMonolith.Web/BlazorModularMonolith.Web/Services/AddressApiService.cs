using System.Net.Http.Json;
using System.Text.Json;
using BlazorModularMonolith.Web.Common;
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

    public async Task<Result<List<AddressModel>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AddressModel>>(BaseEndpoint);
            return Result<List<AddressModel>>.Success(response ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all addresses");
            return Result<List<AddressModel>>.Failure($"Failed to load addresses: {ex.Message}");
        }
    }

    public async Task<Result<AddressModel>> GetByIdAsync(Guid id)
    {
        try
        {
            var address = await _httpClient.GetFromJsonAsync<AddressModel>($"{BaseEndpoint}/{id}");
            return address is not null
                ? Result<AddressModel>.Success(address)
                : Result<AddressModel>.Failure("Address not found.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Result<AddressModel>.Failure("Address not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching address {AddressId}", id);
            return Result<AddressModel>.Failure($"Failed to load address: {ex.Message}");
        }
    }

    public async Task<Result<List<AddressModel>>> GetByOwnerIdAsync(Guid ownerId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AddressModel>>($"{BaseEndpoint}/owner/{ownerId}");
            return Result<List<AddressModel>>.Success(response ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching addresses for owner {OwnerId}", ownerId);
            return Result<List<AddressModel>>.Failure($"Failed to load addresses: {ex.Message}");
        }
    }

    public async Task<Result<List<AddressModel>>> GetByTypeAsync(AddressType type)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AddressModel>>($"{BaseEndpoint}/type/{type}");
            return Result<List<AddressModel>>.Success(response ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching addresses of type {Type}", type);
            return Result<List<AddressModel>>.Failure($"Failed to load addresses: {ex.Message}");
        }
    }

    public async Task<Result<AddressModel>> CreateAsync(CreateAddressModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<AddressModel>.Failure(error ?? "Failed to create address.");
            }
            var address = await response.Content.ReadFromJsonAsync<AddressModel>();
            return address is not null
                ? Result<AddressModel>.Success(address)
                : Result<AddressModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating address");
            return Result<AddressModel>.Failure($"Failed to create address: {ex.Message}");
        }
    }

    public async Task<Result<AddressModel>> UpdateAsync(Guid id, CreateAddressModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<AddressModel>.Failure(error ?? "Failed to update address.");
            }
            var address = await response.Content.ReadFromJsonAsync<AddressModel>();
            return address is not null
                ? Result<AddressModel>.Success(address)
                : Result<AddressModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address {AddressId}", id);
            return Result<AddressModel>.Failure($"Failed to update address: {ex.Message}");
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
                return Result.Failure(error ?? "Failed to delete address.");
            }
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address {AddressId}", id);
            return Result.Failure($"Failed to delete address: {ex.Message}");
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
