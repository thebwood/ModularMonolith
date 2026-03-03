using BlazorModularMonolith.Api.Modules.Authentication.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Authentication.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

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

        // User management (Admin only)
        var usersGroup = endpoints.MapGroup("/api/v{version:apiVersion}/users")
            .WithApiVersionSet(versionSet)
            .WithTags("User Management")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        usersGroup.MapGet("/", async ([FromServices] IUserManagementService userService) =>
        {
            var users = await userService.GetAllUsersAsync();
            return Results.Ok(users);
        })
        .WithName("GetAllUsers")
        .WithSummary("Get all users")
        .Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK);

        usersGroup.MapGet("/{id:guid}", async (Guid id, [FromServices] IUserManagementService userService) =>
        {
            var user = await userService.GetUserByIdAsync(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        })
        .WithName("GetUserById")
        .WithSummary("Get a user by ID")
        .Produces<UserDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        usersGroup.MapPost("/", async (
            [FromBody] CreateUserRequest request,
            [FromServices] IUserManagementService userService) =>
        {
            var user = await userService.CreateUserAsync(request);
            return user is null
                ? Results.BadRequest(new { message = "Username already exists" })
                : Results.Created($"/api/v1/users/{user.Id}", user);
        })
        .WithName("CreateUser")
        .WithSummary("Create a new user")
        .Produces<UserDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        usersGroup.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateUserRequest request,
            [FromServices] IUserManagementService userService) =>
        {
            var user = await userService.UpdateUserAsync(id, request);
            return user is null ? Results.NotFound() : Results.Ok(user);
        })
        .WithName("UpdateUser")
        .WithSummary("Update an existing user")
        .Produces<UserDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        usersGroup.MapDelete("/{id:guid}", async (Guid id, [FromServices] IUserManagementService userService) =>
        {
            var deleted = await userService.DeleteUserAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteUser")
        .WithSummary("Delete a user")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
