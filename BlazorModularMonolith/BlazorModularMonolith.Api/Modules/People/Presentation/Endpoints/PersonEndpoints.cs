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
        var result = await service.GetAllPeopleAsync();
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error);
    }

    private static async Task<IResult> GetPersonById(Guid id, IPersonService service)
    {
        var result = await service.GetPersonAsync(id);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> GetPersonByEmail(string email, IPersonService service)
    {
        var result = await service.GetPersonByEmailAsync(email);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> CreatePerson([FromBody] CreatePersonRequest request, IPersonService service)
    {
        var result = await service.CreatePersonAsync(request);
        return result.IsSuccess
            ? Results.Created($"/api/people/{result.Value!.Id}", result.Value)
            : Results.BadRequest(new { message = result.Error });
    }

    private static async Task<IResult> UpdatePerson(Guid id, [FromBody] UpdatePersonRequest request, IPersonService service)
    {
        var result = await service.UpdatePersonAsync(id, request);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> DeletePerson(Guid id, IPersonService service)
    {
        var result = await service.DeletePersonAsync(id);
        return result.IsSuccess ? Results.NoContent() : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> AddAddressToPerson(Guid id, Guid addressId, IPersonService service)
    {
        var result = await service.AddAddressToPersonAsync(id, addressId);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> RemoveAddressFromPerson(Guid id, Guid addressId, IPersonService service)
    {
        var result = await service.RemoveAddressFromPersonAsync(id, addressId);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }
}
