using BlazorModularMonolith.Api.Modules.Addresses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Addresses.Application.Services;
using BlazorModularMonolith.Api.Modules.Addresses.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlazorModularMonolith.Api.Modules.Addresses.Presentation.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v{version:apiVersion}/addresses")
            .WithTags("Addresses")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetAllAddresses)
            .WithName("GetAllAddresses")
            .WithSummary("Get all addresses");

        group.MapGet("/{id:guid}", GetAddressById)
            .WithName("GetAddressById")
            .WithSummary("Get address by ID");

        group.MapGet("/owner/{ownerId:guid}", GetAddressesByOwner)
            .WithName("GetAddressesByOwner")
            .WithSummary("Get addresses by owner ID");

        group.MapGet("/type/{type}", GetAddressesByType)
            .WithName("GetAddressesByType")
            .WithSummary("Get addresses by type");

        group.MapPost("/", CreateAddress)
            .WithName("CreateAddress")
            .WithSummary("Create a new address");

        group.MapPut("/{id:guid}", UpdateAddress)
            .WithName("UpdateAddress")
            .WithSummary("Update an existing address");

        group.MapDelete("/{id:guid}", DeleteAddress)
            .WithName("DeleteAddress")
            .WithSummary("Delete an address");

        return endpoints;
    }

    private static async Task<IResult> GetAllAddresses(IAddressService service)
    {
        var addresses = await service.GetAllAddressesAsync();
        return Results.Ok(addresses);
    }

    private static async Task<IResult> GetAddressById(Guid id, IAddressService service)
    {
        var address = await service.GetAddressAsync(id);
        return address != null ? Results.Ok(address) : Results.NotFound();
    }

    private static async Task<IResult> GetAddressesByOwner(Guid ownerId, IAddressService service)
    {
        var addresses = await service.GetAddressesByOwnerAsync(ownerId);
        return Results.Ok(addresses);
    }

    private static async Task<IResult> GetAddressesByType(string type, IAddressService service)
    {
        if (!Enum.TryParse<AddressType>(type, true, out var addressType))
        {
            return Results.BadRequest("Invalid address type. Valid values are: Person, Business, Other");
        }

        var addresses = await service.GetAddressesByTypeAsync(addressType);
        return Results.Ok(addresses);
    }

    private static async Task<IResult> CreateAddress([FromBody] CreateAddressRequest request, IAddressService service)
    {
        var created = await service.CreateAddressAsync(request);
        return Results.Created($"/api/addresses/{created.Id}", created);
    }

    private static async Task<IResult> UpdateAddress(Guid id, [FromBody] UpdateAddressRequest request, IAddressService service)
    {
        var updated = await service.UpdateAddressAsync(id, request);
        return updated != null ? Results.Ok(updated) : Results.NotFound();
    }

    private static async Task<IResult> DeleteAddress(Guid id, IAddressService service)
    {
        var deleted = await service.DeleteAddressAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
