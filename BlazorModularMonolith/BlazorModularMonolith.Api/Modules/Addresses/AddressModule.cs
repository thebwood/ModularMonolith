using BlazorModularMonolith.Api.Modules.Addresses.Application.Services;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Repositories;
using BlazorModularMonolith.Api.Modules.Addresses.Infrastructure.Repositories;
using BlazorModularMonolith.Api.Modules.Addresses.Presentation.Endpoints;

namespace BlazorModularMonolith.Api.Modules.Addresses;

public static class AddressModule
{
    public static IServiceCollection AddAddressModule(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, FileAddressRepository>();
        services.AddScoped<IAddressService, AddressService>();

        return services;
    }

    public static IEndpointRouteBuilder MapAddressModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAddressEndpoints();
        return endpoints;
    }
}
