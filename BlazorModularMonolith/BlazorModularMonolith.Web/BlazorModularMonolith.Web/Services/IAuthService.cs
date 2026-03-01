using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IAuthService
{
    Task<UserInfo?> LoginAsync(LoginModel loginModel);
    Task<UserInfo?> RegisterAsync(dynamic registerModel);
    Task LogoutAsync();
    Task<UserInfo?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    string? GetToken();
}
