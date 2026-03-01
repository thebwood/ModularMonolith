using BlazorModularMonolith.Api.Modules.Addresses;
using BlazorModularMonolith.Api.Modules.People;
using BlazorModularMonolith.Api.Modules.Businesses;
using BlazorModularMonolith.Api.Modules.Authentication;
using BlazorModularMonolith.Api.Shared.Extensions;
using Scalar.AspNetCore;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtSettings:SecretKey"]!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7189", "http://localhost:5189")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddAuthenticationModule();
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

app.UseCors("AllowBlazorApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticationModule();
app.MapAddressModule();
app.MapPeopleModule();
app.MapBusinessesModule();

app.MapGet("/", () => Results.Ok(new 
{ 
    Message = "Address Management API - Modular Monolith", 
    Version = "2.0",
    ApiVersion = "v1",
    Modules = new[] { "Addresses", "People", "Businesses", "Authentication" },
    Endpoints = new[] { "/api/v1/addresses", "/api/v1/people", "/api/v1/businesses", "/api/v1/auth" },
    Documentation = "/scalar/v1"
}))
.WithName("GetRoot")
.WithTags("General")
.ExcludeFromDescription();

app.Run();
