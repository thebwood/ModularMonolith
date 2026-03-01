using BlazorModularMonolith.Api.Modules.Authentication.Application.Services;
using BlazorModularMonolith.Api.Modules.Authentication.Domain.Repositories;
using BlazorModularMonolith.Api.Modules.Authentication.Infrastructure.Repositories;
using BlazorModularMonolith.Api.Modules.Authentication.Presentation.Endpoints;

namespace BlazorModularMonolith.Api.Modules.Authentication;

public static class AuthenticationModule
{
    public static IServiceCollection AddAuthenticationModule(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, FileUserRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }

    public static IEndpointRouteBuilder MapAuthenticationModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthenticationEndpoints();
        return endpoints;
    }
}
