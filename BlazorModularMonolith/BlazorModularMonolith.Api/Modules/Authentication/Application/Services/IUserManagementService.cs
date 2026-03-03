using BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;

namespace BlazorModularMonolith.Api.Modules.Authentication.Application.Services;

public interface IUserManagementService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> CreateUserAsync(CreateUserRequest request);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(Guid id);
}
