using System.Net.Http.Json;
using System.Text.Json;
using BlazorModularMonolith.Web.Models;
using Microsoft.JSInterop;

namespace BlazorModularMonolith.Web.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly ITokenProvider _tokenProvider;
    private UserInfo? _currentUser;
    private const string TokenKey = "authToken";
    private const string UserKey = "userData";

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, ITokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _tokenProvider = tokenProvider;
    }

    public async Task<UserInfo?> LoginAsync(LoginModel loginModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/login", new
            {
                username = loginModel.Username,
                password = loginModel.Password
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Login failed with status {response.StatusCode}: {errorContent}");
                return null;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null)
                return null;

            var userInfo = new UserInfo
            {
                Token = loginResponse.Token,
                Username = loginResponse.Username,
                Email = loginResponse.Email,
                Roles = loginResponse.Roles,
                ExpiresAt = loginResponse.ExpiresAt
            };

            await SaveUserInfoAsync(userInfo);
            return userInfo;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login exception: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<UserInfo?> RegisterAsync(RegisterModel registerModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/register", new
            {
                username = registerModel.Username,
                password = registerModel.Password,
                email = registerModel.Email
            });

            if (!response.IsSuccessStatusCode)
                return null;

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null)
                return null;

            var userInfo = new UserInfo
            {
                Token = loginResponse.Token,
                Username = loginResponse.Username,
                Email = loginResponse.Email,
                Roles = loginResponse.Roles,
                ExpiresAt = loginResponse.ExpiresAt
            };

            await SaveUserInfoAsync(userInfo);
            return userInfo;
        }
        catch
        {
            return null;
        }
    }

    private async Task SaveUserInfoAsync(UserInfo userInfo)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, userInfo.Token);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserKey, JsonSerializer.Serialize(userInfo));

        _currentUser = userInfo;
        _tokenProvider.SetToken(userInfo.Token);
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserKey);
        _currentUser = null;
        _tokenProvider.ClearToken();
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        try
        {
            var userDataJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", UserKey);
            if (string.IsNullOrEmpty(userDataJson))
                return null;

            _currentUser = JsonSerializer.Deserialize<UserInfo>(userDataJson);

            if (_currentUser != null && _currentUser.ExpiresAt < DateTime.UtcNow)
            {
                await LogoutAsync();
                return null;
            }

            // Update token provider
            if (_currentUser != null)
            {
                _tokenProvider.SetToken(_currentUser.Token);
            }

            return _currentUser;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetCurrentUserAsync();
        return user != null;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var user = await GetCurrentUserAsync();
        return user?.Roles.Contains(role, StringComparer.OrdinalIgnoreCase) ?? false;
    }

    public string? GetToken()
    {
        return _currentUser?.Token;
    }

    private record LoginResponse(string Token, string Username, string Email, List<string> Roles, DateTime ExpiresAt);
}
