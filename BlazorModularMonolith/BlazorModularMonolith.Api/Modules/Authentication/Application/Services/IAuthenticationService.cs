using BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;

namespace BlazorModularMonolith.Api.Modules.Authentication.Application.Services;

public interface IAuthenticationService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RegisterAsync(RegisterRequest request);
    Task<bool> ValidateTokenAsync(string token);
}
