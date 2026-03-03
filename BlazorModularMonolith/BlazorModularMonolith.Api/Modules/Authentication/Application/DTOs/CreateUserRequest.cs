namespace BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;

public record CreateUserRequest(string Username, string Password, string Email, List<string> Roles);
