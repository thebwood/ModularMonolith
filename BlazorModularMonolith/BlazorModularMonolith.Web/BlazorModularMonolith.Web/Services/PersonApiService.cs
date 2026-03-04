using System.Net.Http.Json;
using System.Text.Json;
using BlazorModularMonolith.Web.Common;
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

    public async Task<Result<List<PersonModel>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PersonModel>>(BaseEndpoint);
            return Result<List<PersonModel>>.Success(response ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all people");
            return Result<List<PersonModel>>.Failure($"Failed to load people: {ex.Message}");
        }
    }

    public async Task<Result<PersonModel>> GetByIdAsync(Guid id)
    {
        try
        {
            var person = await _httpClient.GetFromJsonAsync<PersonModel>($"{BaseEndpoint}/{id}");
            return person is not null
                ? Result<PersonModel>.Success(person)
                : Result<PersonModel>.Failure("Person not found.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Result<PersonModel>.Failure("Person not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching person {PersonId}", id);
            return Result<PersonModel>.Failure($"Failed to load person: {ex.Message}");
        }
    }

    public async Task<Result<PersonModel>> GetByEmailAsync(string email)
    {
        try
        {
            var person = await _httpClient.GetFromJsonAsync<PersonModel>($"{BaseEndpoint}/email/{email}");
            return person is not null
                ? Result<PersonModel>.Success(person)
                : Result<PersonModel>.Failure("Person not found.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Result<PersonModel>.Failure("Person not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching person by email {Email}", email);
            return Result<PersonModel>.Failure($"Failed to load person: {ex.Message}");
        }
    }

    public async Task<Result<PersonModel>> CreateAsync(CreatePersonModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<PersonModel>.Failure(error ?? "Failed to create person.");
            }
            var person = await response.Content.ReadFromJsonAsync<PersonModel>();
            return person is not null
                ? Result<PersonModel>.Success(person)
                : Result<PersonModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person");
            return Result<PersonModel>.Failure($"Failed to create person: {ex.Message}");
        }
    }

    public async Task<Result<PersonModel>> UpdateAsync(Guid id, CreatePersonModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<PersonModel>.Failure(error ?? "Failed to update person.");
            }
            var person = await response.Content.ReadFromJsonAsync<PersonModel>();
            return person is not null
                ? Result<PersonModel>.Success(person)
                : Result<PersonModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person {PersonId}", id);
            return Result<PersonModel>.Failure($"Failed to update person: {ex.Message}");
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
                return Result.Failure(error ?? "Failed to delete person.");
            }
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting person {PersonId}", id);
            return Result.Failure($"Failed to delete person: {ex.Message}");
        }
    }

    public async Task<Result<PersonModel>> AddAddressAsync(Guid personId, Guid addressId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"{BaseEndpoint}/{personId}/addresses/{addressId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<PersonModel>.Failure(error ?? "Failed to add address to person.");
            }
            var person = await response.Content.ReadFromJsonAsync<PersonModel>();
            return person is not null
                ? Result<PersonModel>.Success(person)
                : Result<PersonModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address {AddressId} to person {PersonId}", addressId, personId);
            return Result<PersonModel>.Failure($"Failed to add address: {ex.Message}");
        }
    }

    public async Task<Result<PersonModel>> RemoveAddressAsync(Guid personId, Guid addressId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{personId}/addresses/{addressId}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<PersonModel>.Failure(error ?? "Failed to remove address from person.");
            }
            var person = await response.Content.ReadFromJsonAsync<PersonModel>();
            return person is not null
                ? Result<PersonModel>.Success(person)
                : Result<PersonModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing address {AddressId} from person {PersonId}", addressId, personId);
            return Result<PersonModel>.Failure($"Failed to remove address: {ex.Message}");
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
