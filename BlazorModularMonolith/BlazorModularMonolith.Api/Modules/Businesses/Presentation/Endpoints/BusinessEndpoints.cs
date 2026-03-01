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
            .WithOpenApi();

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
        var businesses = await service.GetAllBusinessesAsync();
        return Results.Ok(businesses);
    }

    private static async Task<IResult> GetBusinessById(Guid id, IBusinessService service)
    {
        var business = await service.GetBusinessAsync(id);
        return business != null ? Results.Ok(business) : Results.NotFound();
    }

    private static async Task<IResult> GetBusinessByTaxId(string taxId, IBusinessService service)
    {
        var business = await service.GetBusinessByTaxIdAsync(taxId);
        return business != null ? Results.Ok(business) : Results.NotFound();
    }

    private static async Task<IResult> GetBusinessesByType(string type, IBusinessService service)
    {
        if (!Enum.TryParse<BusinessType>(type, true, out var businessType))
        {
            return Results.BadRequest("Invalid business type. Valid values are: SoleProprietorship, Partnership, Corporation, LLC, NonProfit, Other");
        }

        var businesses = await service.GetBusinessesByTypeAsync(businessType);
        return Results.Ok(businesses);
    }

    private static async Task<IResult> CreateBusiness([FromBody] CreateBusinessRequest request, IBusinessService service)
    {
        var created = await service.CreateBusinessAsync(request);
        return Results.Created($"/api/businesses/{created.Id}", created);
    }

    private static async Task<IResult> UpdateBusiness(Guid id, [FromBody] UpdateBusinessRequest request, IBusinessService service)
    {
        var updated = await service.UpdateBusinessAsync(id, request);
        return updated != null ? Results.Ok(updated) : Results.NotFound();
    }

    private static async Task<IResult> DeleteBusiness(Guid id, IBusinessService service)
    {
        var deleted = await service.DeleteBusinessAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> AddAddressToBusiness(Guid id, Guid addressId, IBusinessService service)
    {
        var updated = await service.AddAddressToBusinessAsync(id, addressId);
        return updated != null ? Results.Ok(updated) : Results.NotFound();
    }

    private static async Task<IResult> RemoveAddressFromBusiness(Guid id, Guid addressId, IBusinessService service)
    {
        var updated = await service.RemoveAddressFromBusinessAsync(id, addressId);
        return updated != null ? Results.Ok(updated) : Results.NotFound();
    }
}
