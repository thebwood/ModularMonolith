using BlazorModularMonolith.Web.Common;
using BlazorModularMonolith.Web.Models;

namespace BlazorModularMonolith.Web.Services;

public interface IUserApiService
{
    Task<Result<List<UserModel>>> GetAllUsersAsync();
    Task<Result<UserModel>> GetUserByIdAsync(Guid id);
    Task<Result<UserModel>> CreateUserAsync(CreateUserModel model);
    Task<Result<UserModel>> UpdateUserAsync(Guid id, UpdateUserModel model);
    Task<Result> DeleteUserAsync(Guid id);
}
