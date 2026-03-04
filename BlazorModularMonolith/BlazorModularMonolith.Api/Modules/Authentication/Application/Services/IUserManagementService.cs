using BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;
using BlazorModularMonolith.Api.Shared.Common;

namespace BlazorModularMonolith.Api.Modules.Authentication.Application.Services;

public interface IUserManagementService
{
    Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync();
    Task<Result<UserDto>> GetUserByIdAsync(Guid id);
    Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request);
    Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<Result> DeleteUserAsync(Guid id);
}
