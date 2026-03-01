# API Versioning Guide

## Overview

This API uses **URL Path Versioning** to manage different versions of endpoints. This is the most explicit and commonly recommended approach for RESTful APIs.

## Current Version

- **Version**: `v1` (1.0)
- **Status**: Active
- **Base URL Pattern**: `/api/v1/{resource}`

## Versioning Strategy

### URL Path Versioning (Implemented)

All endpoints include the version in the URL path:

```
https://localhost:7188/api/v1/addresses
https://localhost:7188/api/v1/people
https://localhost:7188/api/v1/businesses
```

**Benefits**:
- ✅ Explicit and visible in URLs
- ✅ Easy to test and debug
- ✅ Browser-friendly (can bookmark versions)
- ✅ Works well with caching
- ✅ Clear in API documentation

## Configuration

### Packages Used

```xml
<PackageReference Include="Asp.Versioning.Http" Version="8.1.0" />
<PackageReference Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.1.0" />
```

### Program.cs Configuration

```csharp
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
```

### Endpoint Configuration

Each module's endpoints use the versioned route:

```csharp
var group = endpoints.MapGroup("/api/v{version:apiVersion}/addresses")
    .WithTags("Addresses")
    .WithOpenApi();
```

## Version Headers

The API automatically includes version information in response headers:

### Response Headers
- `api-supported-versions`: Lists all currently supported API versions
- `api-deprecated-versions`: Lists deprecated versions (if any)

### Example
```http
GET /api/v1/addresses
Response Headers:
  api-supported-versions: 1.0
```

## Version Discovery

### Check Current Version
```bash
curl -I https://localhost:7188/api/v1/addresses
```

Look for the `api-supported-versions` header in the response.

### Root Endpoint
```bash
curl https://localhost:7188/
```

Returns API information including supported versions.

## Adding a New Version

When you need to introduce breaking changes, create a new version:

### 1. Update Version Set in Program.cs

```csharp
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))  // Add v2
    .ReportApiVersions()
    .Build();
```

### 2. Create New Endpoints (if needed)

Option A: Same endpoints, different behavior
```csharp
// AddressEndpoints.cs - version specific logic
public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder endpoints)
{
    var group = endpoints.MapGroup("/api/v{version:apiVersion}/addresses")
        .WithTags("Addresses")
        .WithOpenApi();

    group.MapGet("/", GetAllAddresses)
        .MapToApiVersion(1.0)
        .MapToApiVersion(2.0);  // Support both versions
}
```

Option B: Different endpoints for different versions
```csharp
// Create AddressEndpointsV2.cs
var groupV2 = endpoints.MapGroup("/api/v{version:apiVersion}/addresses")
    .WithTags("Addresses")
    .WithOpenApi();

groupV2.MapGet("/", GetAllAddressesV2)
    .MapToApiVersion(2.0);  // v2 only
```

### 3. Version-Specific Logic

```csharp
private static async Task<IResult> GetAllAddresses(
    IAddressService service,
    ApiVersion apiVersion)
{
    var addresses = await service.GetAllAddressesAsync();
    
    if (apiVersion.MajorVersion == 1)
    {
        // v1 response format
        return Results.Ok(addresses);
    }
    else if (apiVersion.MajorVersion == 2)
    {
        // v2 response format (e.g., different DTO structure)
        var v2Addresses = addresses.Select(MapToV2Format);
        return Results.Ok(v2Addresses);
    }
    
    return Results.BadRequest("Unsupported version");
}
```

## Deprecating a Version

When a version becomes outdated:

### 1. Mark as Deprecated

```csharp
var versionSet = app.NewApiVersionSet()
    .HasDeprecatedApiVersion(new ApiVersion(1, 0))  // Mark v1 as deprecated
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();
```

### 2. Set Sunset Date

Add deprecation notices in your documentation and consider adding a custom header:

```csharp
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/v1"))
    {
        context.Response.Headers.Append("Sunset", "Sun, 01 Jan 2025 00:00:00 GMT");
        context.Response.Headers.Append("Deprecation", "true");
    }
    await next();
});
```

### 3. Notify Consumers

- Update documentation
- Add deprecation warnings to Swagger/Scalar
- Email/notify API consumers
- Provide migration guide to newer version

### 4. Remove Old Version

After sufficient notice period:

```csharp
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(2, 0))  // v1 removed
    .ReportApiVersions()
    .Build();
```

## Best Practices

### DO ✅

1. **Maintain Backward Compatibility**: Try to avoid breaking changes within a version
2. **Use Semantic Versioning**: Major version for breaking changes, minor for additions
3. **Document Changes**: Keep a changelog of what changed between versions
4. **Support Multiple Versions**: Maintain at least N-1 versions
5. **Sunset Policy**: Define how long you'll support old versions
6. **Version DTOs**: Create separate DTOs for different versions when needed

### DON'T ❌

1. **Don't break existing versions**: Once published, v1 should remain stable
2. **Don't version too frequently**: Bundle changes into meaningful releases
3. **Don't skip versions**: Go from v1 to v2, not v1 to v3
4. **Don't remove versions without notice**: Give consumers time to migrate
5. **Don't use version for feature flags**: Use proper feature management

## Migration Guide (v1 to v2 Example)

When you eventually create v2, provide a migration guide:

### What Changed in v2

```markdown
### Breaking Changes
- Address DTOs now include `coordinates` field (required)
- Date formats changed from ISO 8601 to Unix timestamps
- Error responses restructured

### New Features
- Batch operations for addresses
- Filtering with OData syntax
- Pagination with cursor-based navigation

### Migration Steps
1. Update base URL from `/api/v1/` to `/api/v2/`
2. Add `coordinates` to all address creation requests
3. Update date parsing logic
4. Handle new error response format
```

## Testing Different Versions

### Using curl
```bash
# v1
curl https://localhost:7188/api/v1/addresses

# v2 (when available)
curl https://localhost:7188/api/v2/addresses
```

### Using Scalar/Swagger
The interactive documentation automatically shows all available versions and allows you to switch between them.

## Alternative Versioning Strategies (Not Implemented)

### Query String Versioning
```
/api/addresses?api-version=1.0
/api/addresses?api-version=2.0
```

### Header Versioning
```
GET /api/addresses
Header: api-version: 1.0
```

### Media Type Versioning
```
Accept: application/json; version=1.0
Accept: application/json; version=2.0
```

**Why URL Path Versioning?**
- Most visible and explicit
- Best for RESTful APIs
- Industry standard (used by GitHub, Stripe, Twitter APIs)
- Works well with all HTTP clients
- Clear in documentation

## Version Lifecycle

```
v1.0 → Active (Current)
  ↓
v2.0 → Released (v1.0 marked deprecated)
  ↓
v1.0 → Sunset announced (6 months notice)
  ↓
v1.0 → Removed (after sunset date)
```

## Monitoring Version Usage

Track which versions are being used:

```csharp
app.Use(async (context, next) =>
{
    var apiVersion = context.GetRequestedApiVersion();
    // Log version usage for analytics
    _logger.LogInformation("API Version {Version} called", apiVersion);
    await next();
});
```

## Resources

- [ASP.NET Core API Versioning Documentation](https://github.com/dotnet/aspnet-api-versioning)
- [Microsoft REST API Guidelines - Versioning](https://github.com/microsoft/api-guidelines/blob/vNext/Guidelines.md#12-versioning)
- [Semantic Versioning](https://semver.org/)
