# API Documentation Configuration

## Scalar API Documentation

This project uses **Scalar** - a modern, beautiful API documentation UI that's built specifically for OpenAPI 3.0+ specifications.

## Accessing the Documentation

When you run the application in Development mode:

```bash
dotnet run
```

The browser will **automatically open** to:
- **HTTPS**: `https://localhost:7188/scalar/v1`
- **HTTP**: `http://localhost:5137/scalar/v1`

## Available Documentation Endpoints

| Endpoint | Description |
|----------|-------------|
| `/scalar/v1` | Interactive Scalar UI (Development only) |
| `/openapi/v1.json` | Raw OpenAPI 3.0 specification |
| `/` | API information and module list |

## Features

### Scalar UI Benefits
- ✅ **Interactive Testing** - Test endpoints directly in the browser
- ✅ **Request Examples** - Pre-populated request examples for all endpoints
- ✅ **Response Schemas** - Clear documentation of response structures
- ✅ **Authentication Support** - Easy authentication configuration
- ✅ **Dark/Light Themes** - Currently configured with Purple theme
- ✅ **Sidebar Navigation** - Easy navigation between endpoints
- ✅ **Modern Design** - Beautiful, clean interface

### Configuration

The Scalar UI is configured in `Program.cs`:

```csharp
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
```

### Launch Settings

The `Properties/launchSettings.json` is configured to:
- Open browser automatically (`"launchBrowser": true`)
- Navigate to Scalar UI (`"launchUrl": "scalar/v1"`)
- Work with both HTTP and HTTPS profiles

## Customization Options

### Available Themes
- `ScalarTheme.Default`
- `ScalarTheme.Alternate`
- `ScalarTheme.Moon`
- `ScalarTheme.Purple` (current)
- `ScalarTheme.Solarized`
- `ScalarTheme.BluePlanet`
- `ScalarTheme.Saturn`
- `ScalarTheme.Kepler`
- `ScalarTheme.Mars`
- `ScalarTheme.DeepSpace`

### Changing Themes

Edit `Program.cs`:
```csharp
app.MapScalarApiReference(options =>
{
    options.Title = "Address Management API";
    options.Theme = ScalarTheme.DeepSpace; // Change this
    options.ShowSidebar = true;
});
```

### Additional Configuration Options

```csharp
app.MapScalarApiReference(options =>
{
    options.Title = "Address Management API";
    options.Theme = ScalarTheme.Purple;
    options.ShowSidebar = true;
    options.DarkMode = false; // Force light mode
    options.DefaultHttpClient = ScalarTarget.CSharp; // Default code examples
});
```

## Disabling Automatic Browser Launch

If you prefer not to launch the browser automatically:

1. Open `Properties/launchSettings.json`
2. Set `"launchBrowser": false` in the profile you use
3. Manually navigate to `https://localhost:7188/scalar/v1` when needed

## Production Considerations

The Scalar UI is **only enabled in Development mode** by default. This is intentional:
- Production APIs typically don't expose interactive documentation
- Reduces attack surface
- Improves performance

To enable in other environments, modify the condition in `Program.cs`:

```csharp
// Enable in Development and Staging
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Enable in all environments (not recommended)
app.MapOpenApi();
app.MapScalarApiReference();
```

## Alternative: Swagger UI

If you prefer the traditional Swagger UI, you can replace Scalar with Swashbuckle:

1. Add package:
```bash
dotnet add package Swashbuckle.AspNetCore
```

2. Update `Program.cs`:
```csharp
// Replace AddOpenApi with:
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Replace MapOpenApi and MapScalarApiReference with:
app.UseSwagger();
app.UseSwaggerUI();
```

## Testing the API

Once the Scalar UI is open:

1. **Browse endpoints** - Use the sidebar to navigate
2. **Expand an endpoint** - Click to see details
3. **Try it out** - Click "Send" to test endpoints
4. **View schemas** - See request/response models
5. **Copy curl commands** - Get ready-to-use curl commands

## Troubleshooting

### Scalar UI doesn't open
- Check that you're running in Development mode
- Verify the URL in launchSettings.json matches your needs
- Check console output for the actual URLs being used

### OpenAPI spec not found
- Ensure `app.MapOpenApi()` is called before `app.MapScalarApiReference()`
- Check that OpenAPI package is installed

### Browser opens but shows error
- Wait a few seconds for the API to fully start
- Manually refresh the browser
- Check the console for startup errors

## Benefits Over Traditional Swagger

1. **Modern Design** - Cleaner, more intuitive interface
2. **Better Performance** - Faster rendering of large APIs
3. **Code Generation** - Built-in code examples in multiple languages
4. **Better Mobile Support** - Responsive design
5. **Markdown Support** - Rich text in descriptions
6. **Better Search** - Faster endpoint search

## Learn More

- [Scalar Documentation](https://github.com/scalar/scalar)
- [ASP.NET Core OpenAPI](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/)
- [OpenAPI Specification](https://spec.openapis.org/oas/latest.html)
