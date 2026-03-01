using BlazorModularMonolith.Api.Modules.People.Application.DTOs;
using BlazorModularMonolith.Api.Modules.People.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorModularMonolith.Api.Modules.People.Presentation.Endpoints;

public static class PersonEndpoints
{
    public static IEndpointRouteBuilder MapPersonEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v{version:apiVersion}/people")
            .WithTags("People")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetAllPeople)
            .WithName("GetAllPeople")
            .WithSummary("Get all people");

        group.MapGet("/{id:guid}", GetPersonById)
            .WithName("GetPersonById")
            .WithSummary("Get person by ID");

        group.MapGet("/email/{email}", GetPersonByEmail)
            .WithName("GetPersonByEmail")
            .WithSummary("Get person by email");

        group.MapPost("/", CreatePerson)
            .WithName("CreatePerson")
            .WithSummary("Create a new person");

        group.MapPut("/{id:guid}", UpdatePerson)
            .WithName("UpdatePerson")
            .WithSummary("Update an existing person");

        group.MapDelete("/{id:guid}", DeletePerson)
            .WithName("DeletePerson")
            .WithSummary("Delete a person");

        group.MapPost("/{id:guid}/addresses/{addressId:guid}", AddAddressToPerson)
            .WithName("AddAddressToPerson")
            .WithSummary("Add an address to a person");

        group.MapDelete("/{id:guid}/addresses/{addressId:guid}", RemoveAddressFromPerson)
            .WithName("RemoveAddressFromPerson")
            .WithSummary("Remove an address from a person");

        return endpoints;
    }

    private static async Task<IResult> GetAllPeople(IPersonService service)
    {
        var people = await service.GetAllPeopleAsync();
        return Results.Ok(people);
    }

    private static async Task<IResult> GetPersonById(Guid id, IPersonService service)
    {
        var person = await service.GetPersonAsync(id);
        return person != null ? Results.Ok(person) : Results.NotFound();
    }

    private static async Task<IResult> GetPersonByEmail(string email, IPersonService service)
    {
        var person = await service.GetPersonByEmailAsync(email);
        return person != null ? Results.Ok(person) : Results.NotFound();
    }

    private static async Task<IResult> CreatePerson([FromBody] CreatePersonRequest request, IPersonService service)
    {
        var created = await service.CreatePersonAsync(request);
        return Results.Created($"/api/people/{created.Id}", created);
    }

    private static async Task<IResult> UpdatePerson(Guid id, [FromBody] UpdatePersonRequest request, IPersonService service)
    {
        var updated = await service.UpdatePersonAsync(id, request);
        return updated != null ? Results.Ok(updated) : Results.NotFound();
    }

    private static async Task<IResult> DeletePerson(Guid id, IPersonService service)
    {
        var deleted = await service.DeletePersonAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> AddAddressToPerson(Guid id, Guid addressId, IPersonService service)
    {
        var updated = await service.AddAddressToPersonAsync(id, addressId);
        return updated != null ? Results.Ok(updated) : Results.NotFound();
    }

    private static async Task<IResult> RemoveAddressFromPerson(Guid id, Guid addressId, IPersonService service)
    {
        var updated = await service.RemoveAddressFromPersonAsync(id, addressId);
        return updated != null ? Results.Ok(updated) : Results.NotFound();
    }
}
