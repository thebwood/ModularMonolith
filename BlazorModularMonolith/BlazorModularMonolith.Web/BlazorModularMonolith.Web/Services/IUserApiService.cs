using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IUserApiService
{
    Task<List<UserModel>> GetAllUsersAsync();
    Task<UserModel?> GetUserByIdAsync(Guid id);
    Task<UserModel?> CreateUserAsync(CreateUserModel model);
    Task<UserModel?> UpdateUserAsync(Guid id, UpdateUserModel model);
    Task<bool> DeleteUserAsync(Guid id);
}
