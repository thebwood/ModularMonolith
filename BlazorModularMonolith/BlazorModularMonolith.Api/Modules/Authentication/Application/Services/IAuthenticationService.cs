using BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;
using BlazorModularMonolith.Api.Shared.Common;

namespace BlazorModularMonolith.Api.Modules.Authentication.Application.Services;

public interface IAuthenticationService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<LoginResponse>> RegisterAsync(RegisterRequest request);
    Task<bool> ValidateTokenAsync(string token);
}
