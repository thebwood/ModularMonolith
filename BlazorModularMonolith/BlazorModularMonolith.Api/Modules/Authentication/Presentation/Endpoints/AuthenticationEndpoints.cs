using BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Authentication.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace BlazorModularMonolith.Api.Modules.Authentication.Presentation.Endpoints;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();

        var group = endpoints.MapGroup("/api/v{version:apiVersion}/auth")
            .WithApiVersionSet(versionSet)
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/login", async (
            [FromBody] LoginRequest request,
            [FromServices] IAuthenticationService authService) =>
        {
            var response = await authService.LoginAsync(request);
            
            if (response == null)
                return Results.Unauthorized();

            return Results.Ok(response);
        })
        .WithName("Login")
        .WithSummary("Authenticate user and receive JWT token")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/register", async (
            [FromBody] RegisterRequest request,
            [FromServices] IAuthenticationService authService) =>
        {
            var response = await authService.RegisterAsync(request);
            
            if (response == null)
                return Results.BadRequest(new { message = "User already exists" });

            return Results.Ok(response);
        })
        .WithName("Register")
        .WithSummary("Register a new user account")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/validate", async (
            HttpContext context,
            [FromServices] IAuthenticationService authService) =>
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest(new { message = "No token provided" });

            var isValid = await authService.ValidateTokenAsync(token);
            
            return Results.Ok(new { isValid });
        })
        .WithName("ValidateToken")
        .WithSummary("Validate a JWT token")
        .Produces<object>(StatusCodes.Status200OK);

        return endpoints;
    }
}
