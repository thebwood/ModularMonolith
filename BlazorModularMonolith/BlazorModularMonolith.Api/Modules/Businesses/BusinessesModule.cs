using BlazorModularMonolith.Api.Modules.Businesses.Application.Services;
using BlazorModularMonolith.Api.Modules.Businesses.Domain.Repositories;
using BlazorModularMonolith.Api.Modules.Businesses.Infrastructure.Repositories;
using BlazorModularMonolith.Api.Modules.Businesses.Presentation.Endpoints;

namespace BlazorModularMonolith.Api.Modules.Businesses;

public static class BusinessesModule
{
    public static IServiceCollection AddBusinessesModule(this IServiceCollection services)
    {
        services.AddScoped<IBusinessRepository, FileBusinessRepository>();
        services.AddScoped<IBusinessService, BusinessService>();

        return services;
    }

    public static IEndpointRouteBuilder MapBusinessesModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapBusinessEndpoints();
        return endpoints;
    }
}
