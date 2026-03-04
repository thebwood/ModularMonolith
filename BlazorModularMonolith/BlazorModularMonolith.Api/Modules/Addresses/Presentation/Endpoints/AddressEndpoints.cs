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
        var result = await service.GetAllAddressesAsync();
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error);
    }

    private static async Task<IResult> GetAddressById(Guid id, IAddressService service)
    {
        var result = await service.GetAddressAsync(id);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> GetAddressesByOwner(Guid ownerId, IAddressService service)
    {
        var result = await service.GetAddressesByOwnerAsync(ownerId);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error);
    }

    private static async Task<IResult> GetAddressesByType(string type, IAddressService service)
    {
        if (!Enum.TryParse<AddressType>(type, true, out var addressType))
        {
            return Results.BadRequest("Invalid address type. Valid values are: Person, Business, Other");
        }

        var result = await service.GetAddressesByTypeAsync(addressType);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error);
    }

    private static async Task<IResult> CreateAddress([FromBody] CreateAddressRequest request, IAddressService service)
    {
        var result = await service.CreateAddressAsync(request);
        return result.IsSuccess
            ? Results.Created($"/api/addresses/{result.Value!.Id}", result.Value)
            : Results.BadRequest(new { message = result.Error });
    }

    private static async Task<IResult> UpdateAddress(Guid id, [FromBody] UpdateAddressRequest request, IAddressService service)
    {
        var result = await service.UpdateAddressAsync(id, request);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> DeleteAddress(Guid id, IAddressService service)
    {
        var result = await service.DeleteAddressAsync(id);
        return result.IsSuccess ? Results.NoContent() : Results.NotFound(new { message = result.Error });
    }
}
