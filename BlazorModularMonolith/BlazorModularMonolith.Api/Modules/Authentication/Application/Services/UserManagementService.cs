using System.Security.Cryptography;
using System.Text;
using BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Authentication.Domain.Entities;
using BlazorModularMonolith.Api.Modules.Authentication.Domain.Repositories;

namespace BlazorModularMonolith.Api.Modules.Authentication.Application.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;

    public UserManagementService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(ToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserRequest request)
    {
        var existing = await _userRepository.GetByUsernameAsync(request.Username);
        if (existing is not null)
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Roles = request.Roles,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);
        return ToDto(user);
    }

    public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return null;

        user.Email = request.Email;
        user.Roles = request.Roles;
        user.IsActive = request.IsActive;

        await _userRepository.UpdateAsync(user);
        return ToDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }

    private static UserDto ToDto(User user) =>
        new(user.Id, user.Username, user.Email, user.Roles, user.CreatedAt, user.LastLoginAt, user.IsActive);

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
