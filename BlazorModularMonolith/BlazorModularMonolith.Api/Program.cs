using BlazorModularMonolith.Api.Modules.Addresses;
using BlazorModularMonolith.Api.Modules.People;
using BlazorModularMonolith.Api.Modules.Businesses;
using BlazorModularMonolith.Api.Shared.Extensions;
using Scalar.AspNetCore;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddOpenApi();

builder.Services.AddAddressModule();
builder.Services.AddPeopleModule();
builder.Services.AddBusinessesModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Address Management API";
        options.Theme = ScalarTheme.Purple;
        options.ShowSidebar = true;
    });
}

app.UseCustomMiddleware();

app.UseHttpsRedirection();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

app.MapAddressModule();
app.MapPeopleModule();
app.MapBusinessesModule();

app.MapGet("/", () => Results.Ok(new 
{ 
    Message = "Address Management API - Modular Monolith", 
    Version = "2.0",
    ApiVersion = "v1",
    Modules = new[] { "Addresses", "People", "Businesses" },
    Endpoints = new[] { "/api/v1/addresses", "/api/v1/people", "/api/v1/businesses" },
    Documentation = "/scalar/v1"
}))
.WithName("GetRoot")
.WithTags("General")
.ExcludeFromDescription();

app.Run();
