using BlazorModularMonolith.Api.Modules.Authentication.Domain.Entities;

namespace BlazorModularMonolith.Api.Modules.Authentication.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task DeleteAsync(Guid id);
}
