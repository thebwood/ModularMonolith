namespace BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;

public record UpdateUserRequest(string Email, List<string> Roles, bool IsActive);
