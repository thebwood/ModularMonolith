using System.Security.Cryptography;
using System.Text;
using BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Authentication.Domain.Entities;
using BlazorModularMonolith.Api.Modules.Authentication.Domain.Repositories;
using BlazorModularMonolith.Api.Shared.Common;

namespace BlazorModularMonolith.Api.Modules.Authentication.Application.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;

    public UserManagementService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return Result<IEnumerable<UserDto>>.Success(users.Select(ToDto));
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null
            ? Result<UserDto>.Failure($"User with ID '{id}' not found.")
            : Result<UserDto>.Success(ToDto(user));
    }

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request)
    {
        var existing = await _userRepository.GetByUsernameAsync(request.Username);
        if (existing is not null)
            return Result<UserDto>.Failure("Username already exists.");

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
        return Result<UserDto>.Success(ToDto(user));
    }

    public async Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return Result<UserDto>.Failure($"User with ID '{id}' not found.");

        user.Email = request.Email;
        user.Roles = request.Roles;
        user.IsActive = request.IsActive;

        await _userRepository.UpdateAsync(user);
        return Result<UserDto>.Success(ToDto(user));
    }

    public async Task<Result> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return Result.Failure($"User with ID '{id}' not found.");

        await _userRepository.DeleteAsync(id);
        return Result.Success();
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
