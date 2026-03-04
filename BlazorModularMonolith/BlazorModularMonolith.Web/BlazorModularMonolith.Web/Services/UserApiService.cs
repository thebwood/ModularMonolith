using System.Net.Http.Json;
using System.Text.Json;
using BlazorModularMonolith.Web.Common;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<List<UserModel>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<UserModel>>("/api/v1/users/") ?? [];
            return Result<List<UserModel>>.Success(users);
        }
        catch (Exception ex)
        {
            return Result<List<UserModel>>.Failure($"Failed to load users: {ex.Message}");
        }
    }

    public async Task<Result<UserModel>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _httpClient.GetFromJsonAsync<UserModel>($"/api/v1/users/{id}");
            return user is not null
                ? Result<UserModel>.Success(user)
                : Result<UserModel>.Failure("User not found.");
        }
        catch (Exception ex)
        {
            return Result<UserModel>.Failure($"Failed to load user: {ex.Message}");
        }
    }

    public async Task<Result<UserModel>> CreateUserAsync(CreateUserModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/users/", new
            {
                username = model.Username,
                password = model.Password,
                email = model.Email,
                roles = model.Roles
            });

            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<UserModel>.Failure(error ?? "Failed to create user.");
            }

            var user = await response.Content.ReadFromJsonAsync<UserModel>();
            return user is not null
                ? Result<UserModel>.Success(user)
                : Result<UserModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            return Result<UserModel>.Failure($"Failed to create user: {ex.Message}");
        }
    }

    public async Task<Result<UserModel>> UpdateUserAsync(Guid id, UpdateUserModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/v1/users/{id}", new
            {
                email = model.Email,
                roles = model.Roles,
                isActive = model.IsActive
            });

            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result<UserModel>.Failure(error ?? "Failed to update user.");
            }

            var user = await response.Content.ReadFromJsonAsync<UserModel>();
            return user is not null
                ? Result<UserModel>.Success(user)
                : Result<UserModel>.Failure("Invalid server response.");
        }
        catch (Exception ex)
        {
            return Result<UserModel>.Failure($"Failed to update user: {ex.Message}");
        }
    }

    public async Task<Result> DeleteUserAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await TryReadErrorAsync(response);
                return Result.Failure(error ?? "Failed to delete user.");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete user: {ex.Message}");
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
