namespace BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    List<string> Roles,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    bool IsActive);
