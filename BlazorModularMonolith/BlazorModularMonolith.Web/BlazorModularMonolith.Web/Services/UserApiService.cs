using System.Net.Http.Json;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<UserModel>>("/api/v1/users/") ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<UserModel?> GetUserByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserModel>($"/api/v1/users/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserModel?> CreateUserAsync(CreateUserModel model)
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
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<UserModel>()
                : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserModel?> UpdateUserAsync(Guid id, UpdateUserModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/v1/users/{id}", new
            {
                email = model.Email,
                roles = model.Roles,
                isActive = model.IsActive
            });
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<UserModel>()
                : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/users/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
