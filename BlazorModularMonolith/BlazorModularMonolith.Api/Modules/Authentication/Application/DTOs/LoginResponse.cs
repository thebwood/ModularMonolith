namespace BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;

public record LoginResponse(string Token, string Username, string Email, List<string> Roles, DateTime ExpiresAt);
