using BlazorModularMonolith.Api.Modules.People.Application.Services;
using BlazorModularMonolith.Api.Modules.People.Domain.Repositories;
using BlazorModularMonolith.Api.Modules.People.Infrastructure.Repositories;
using BlazorModularMonolith.Api.Modules.People.Presentation.Endpoints;

namespace BlazorModularMonolith.Api.Modules.People;

public static class PeopleModule
{
    public static IServiceCollection AddPeopleModule(this IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, FilePersonRepository>();
        services.AddScoped<IPersonService, PersonService>();

        return services;
    }

    public static IEndpointRouteBuilder MapPeopleModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPersonEndpoints();
        return endpoints;
    }
}
