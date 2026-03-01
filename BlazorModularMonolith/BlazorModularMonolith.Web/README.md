# Blazor Web App - Address Management System

A Blazor Web App implementing MVVM pattern to consume the Address Management API.

## Architecture Pattern: MVVM (Model-View-ViewModel)

This Blazor application follows the **MVVM** pattern for clean separation of concerns:

```
Models/              # Data models (DTOs from API)
├── AddressModel.cs
├── PersonModel.cs
└── BusinessModel.cs

Services/            # API consumption layer
├── IAddressApiService.cs
├── AddressApiService.cs
├── IPersonApiService.cs
├── PersonApiService.cs
├── IBusinessApiService.cs
└── BusinessApiService.cs

ViewModels/          # Presentation logic & state management
├── ViewModelBase.cs (Base class with INotifyPropertyChanged)
├── PeopleViewModel.cs
└── BusinessesViewModel.cs

Components/Pages/    # Views (Blazor components)
├── People.razor
├── Businesses.razor
└── ...

Middleware/          # Cross-cutting concerns
├── RequestLoggingMiddleware.cs
└── GlobalExceptionHandlerMiddleware.cs
```

## Features

### ✅ MVVM Implementation
- **Models**: Data transfer objects matching API contracts
- **Views**: Blazor `.razor` components (Pages)
- **ViewModels**: Presentation logic, state management, API orchestration

### ✅ API Integration
- **HttpClient** services for each module (Addresses, People, Businesses)
- **Typed HTTP clients** with dependency injection
- **Error handling** and logging
- **Async/await** patterns throughout

### ✅ Middleware
- **Request Logging**: Logs all HTTP requests with timing
- **Global Exception Handler**: Catches and logs unhandled exceptions

### ✅ Features
- **CRUD operations** for People and Businesses
- **Real-time UI updates** with Blazor Server interactivity
- **Form validation** with EditForm
- **Error messaging** with dismissible alerts
- **Loading states** with spinners
- **Responsive design** with Bootstrap 5

## Project Structure

```
BlazorModularMonolith.Web/
├── Components/
│   ├── Layout/           # App shell (NavMenu, MainLayout)
│   └── Pages/            # Blazor pages/views
│       ├── People.razor
│       ├── Businesses.razor
│       └── ...
├── Models/              # Data models
├── Services/            # HTTP API services
├── ViewModels/          # MVVM ViewModels
├── Middleware/          # Custom middleware
├── Extensions/          # Extension methods
└── Program.cs           # App startup & DI configuration
```

## Running the Application

### Prerequisites
1. **.NET 10 SDK** installed
2. **API must be running** at `https://localhost:7188`

### Start the API First
```bash
cd BlazorModularMonolith.Api
dotnet run
```

### Then Start the Blazor App
```bash
cd BlazorModularMonolith.Web\BlazorModularMonolith.Web
dotnet run
```

The Blazor app will be available at:
- **HTTPS**: `https://localhost:7001` (or as configured)
- **HTTP**: `http://localhost:5001`

## Configuration

### API Base URL
Configure the API base URL in `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7188"
  }
}
```

### HttpClient Configuration
HTTP clients are configured with:
- **Base Address**: From configuration
- **Timeout**: 30 seconds
- **Dependency Injection**: Scoped lifetime

## MVVM Pattern Explained

### Model
Models represent the data structure from the API:

```csharp
public class PersonModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // ... other properties
}
```

### View (Blazor Component)
Views are Blazor `.razor` components that bind to ViewModels:

```razor
@page "/people"
@inject PeopleViewModel ViewModel

<h1>People Management</h1>

@foreach (var person in ViewModel.People)
{
    <div>@person.FullName</div>
}
```

### ViewModel
ViewModels handle presentation logic and API calls:

```csharp
public class PeopleViewModel : ViewModelBase
{
    private readonly IPersonApiService _personService;
    
    public List<PersonModel> People { get; set; }
    public bool IsBusy { get; set; }
    
    public async Task LoadPeopleAsync()
    {
        IsBusy = true;
        People = await _personService.GetAllAsync();
        IsBusy = false;
    }
}
```

## ViewModelBase Features

All ViewModels inherit from `ViewModelBase` which provides:

- **INotifyPropertyChanged**: Property change notifications
- **SetProperty**: Helper for property updates with notifications
- **IsBusy**: Loading state management
- **ErrorMessage**: Error handling
- **HasError**: Error state check

## API Services

### IPersonApiService
```csharp
public interface IPersonApiService
{
    Task<List<PersonModel>> GetAllAsync();
    Task<PersonModel?> GetByIdAsync(Guid id);
    Task<PersonModel> CreateAsync(CreatePersonModel model);
    Task<PersonModel?> UpdateAsync(Guid id, CreatePersonModel model);
    Task<bool> DeleteAsync(Guid id);
}
```

### Usage in ViewModel
```csharp
public class PeopleViewModel
{
    private readonly IPersonApiService _personService;
    
    public async Task<bool> CreatePersonAsync()
    {
        try
        {
            var created = await _personService.CreateAsync(NewPerson);
            People.Add(created);
            return true;
        }
        catch (Exception ex)
        {
            SetError("Failed to create person");
            return false;
        }
    }
}
```

## Middleware

### Request Logging Middleware
Logs all requests with:
- HTTP method and path
- Response status code
- Request duration

### Global Exception Handler
Catches unhandled exceptions and logs them for debugging.

## Pages

### People Page (`/people`)
- View all people
- Create new person
- Edit existing person
- Delete person
- Shows loading states
- Error handling with alerts

### Businesses Page (`/businesses`)
- View all businesses
- Create new business
- Edit existing business
- Delete business
- Filter by business type
- Address management

## Dependency Injection

Services are registered in `Program.cs`:

```csharp
// HTTP Clients for API
builder.Services.AddHttpClient<IAddressApiService, AddressApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ViewModels
builder.Services.AddScoped<PeopleViewModel>();
builder.Services.AddScoped<BusinessesViewModel>();
```

## Benefits of This Architecture

### 1. **Separation of Concerns**
- Models: Pure data
- ViewModels: Presentation logic
- Views: UI rendering
- Services: API communication

### 2. **Testability**
- ViewModels can be unit tested
- Services can be mocked
- No UI dependencies in tests

### 3. **Reusability**
- ViewModels can be shared across multiple views
- Services can be used by multiple ViewModels
- Models match API contracts

### 4. **Maintainability**
- Clear responsibilities
- Easy to locate and fix issues
- Consistent patterns

### 5. **Scalability**
- Easy to add new features
- Can evolve to Blazor WebAssembly
- Can add state management (Fluxor, etc.)

## Future Enhancements

Potential improvements:
- **Validation**: Add DataAnnotations to models
- **State Management**: Add Fluxor or similar
- **Caching**: Add response caching
- **Real-time Updates**: Add SignalR for live updates
- **Pagination**: Add paging for large datasets
- **Search & Filter**: Add advanced filtering
- **Unit Tests**: Add tests for ViewModels and Services
- **Authentication**: Add user authentication
- **Authorization**: Add role-based access

## Troubleshooting

### API Connection Issues
If you get connection errors:
1. Verify API is running at `https://localhost:7188`
2. Check `appsettings.json` has correct URL
3. Check CORS settings on API if needed

### Build Errors
```bash
dotnet clean
dotnet restore
dotnet build
```

### Runtime Errors
Check the browser console (F12) for JavaScript errors and the terminal for server logs.

## Additional Resources

- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [HttpClient Best Practices](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
