using BlazorModularMonolith.Api.Modules.Businesses.Application.DTOs;
using BlazorModularMonolith.Api.Modules.Businesses.Application.Services;
using BlazorModularMonolith.Api.Modules.Businesses.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlazorModularMonolith.Api.Modules.Businesses.Presentation.Endpoints;

public static class BusinessEndpoints
{
    public static IEndpointRouteBuilder MapBusinessEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v{version:apiVersion}/businesses")
            .WithTags("Businesses")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetAllBusinesses)
            .WithName("GetAllBusinesses")
            .WithSummary("Get all businesses");

        group.MapGet("/{id:guid}", GetBusinessById)
            .WithName("GetBusinessById")
            .WithSummary("Get business by ID");

        group.MapGet("/taxid/{taxId}", GetBusinessByTaxId)
            .WithName("GetBusinessByTaxId")
            .WithSummary("Get business by tax ID");

        group.MapGet("/type/{type}", GetBusinessesByType)
            .WithName("GetBusinessesByType")
            .WithSummary("Get businesses by type");

        group.MapPost("/", CreateBusiness)
            .WithName("CreateBusiness")
            .WithSummary("Create a new business");

        group.MapPut("/{id:guid}", UpdateBusiness)
            .WithName("UpdateBusiness")
            .WithSummary("Update an existing business");

        group.MapDelete("/{id:guid}", DeleteBusiness)
            .WithName("DeleteBusiness")
            .WithSummary("Delete a business");

        group.MapPost("/{id:guid}/addresses/{addressId:guid}", AddAddressToBusiness)
            .WithName("AddAddressToBusiness")
            .WithSummary("Add an address to a business");

        group.MapDelete("/{id:guid}/addresses/{addressId:guid}", RemoveAddressFromBusiness)
            .WithName("RemoveAddressFromBusiness")
            .WithSummary("Remove an address from a business");

        return endpoints;
    }

    private static async Task<IResult> GetAllBusinesses(IBusinessService service)
    {
        var result = await service.GetAllBusinessesAsync();
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error);
    }

    private static async Task<IResult> GetBusinessById(Guid id, IBusinessService service)
    {
        var result = await service.GetBusinessAsync(id);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> GetBusinessByTaxId(string taxId, IBusinessService service)
    {
        var result = await service.GetBusinessByTaxIdAsync(taxId);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> GetBusinessesByType(string type, IBusinessService service)
    {
        if (!Enum.TryParse<BusinessType>(type, true, out var businessType))
        {
            return Results.BadRequest("Invalid business type. Valid values are: SoleProprietorship, Partnership, Corporation, LLC, NonProfit, Other");
        }

        var result = await service.GetBusinessesByTypeAsync(businessType);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error);
    }

    private static async Task<IResult> CreateBusiness([FromBody] CreateBusinessRequest request, IBusinessService service)
    {
        var result = await service.CreateBusinessAsync(request);
        return result.IsSuccess
            ? Results.Created($"/api/businesses/{result.Value!.Id}", result.Value)
            : Results.BadRequest(new { message = result.Error });
    }

    private static async Task<IResult> UpdateBusiness(Guid id, [FromBody] UpdateBusinessRequest request, IBusinessService service)
    {
        var result = await service.UpdateBusinessAsync(id, request);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> DeleteBusiness(Guid id, IBusinessService service)
    {
        var result = await service.DeleteBusinessAsync(id);
        return result.IsSuccess ? Results.NoContent() : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> AddAddressToBusiness(Guid id, Guid addressId, IBusinessService service)
    {
        var result = await service.AddAddressToBusinessAsync(id, addressId);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }

    private static async Task<IResult> RemoveAddressFromBusiness(Guid id, Guid addressId, IBusinessService service)
    {
        var result = await service.RemoveAddressFromBusinessAsync(id, addressId);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { message = result.Error });
    }
}
