using BlazorModularMonolith.Web.Common;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IAuthService
{
    Task<Result<UserInfo>> LoginAsync(LoginModel loginModel);
    Task<Result<UserInfo>> RegisterAsync(RegisterModel registerModel);
    Task LogoutAsync();
    Task<UserInfo?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsInRoleAsync(string role);
    string? GetToken();
}
