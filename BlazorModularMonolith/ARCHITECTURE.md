# Architecture Guide - Modular Monolith to Microservices

## Overview

This project demonstrates a **Modular Monolith** architecture that is designed to evolve into **Microservices** when needed. It follows **Clean Architecture** principles to maintain clear boundaries and separation of concerns.

## Clean Architecture Layers

### 1. Domain Layer (Innermost)
**Location**: `Modules/{ModuleName}/Domain/`

**Purpose**: Core business logic and entities
- Pure C# classes with no external dependencies
- Domain entities (e.g., `Person`, `Business`, `Address`)
- Repository interfaces (contracts, not implementations)
- Domain events
- Value objects

**Rules**:
- ❌ No dependencies on other layers
- ❌ No framework dependencies
- ✅ Pure business logic only
- ✅ Can reference other Domain entities within the same module

**Example**:
```csharp
// Domain/Entities/Person.cs
public class Person
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // No references to Infrastructure or Application
}
```

### 2. Application Layer
**Location**: `Modules/{ModuleName}/Application/`

**Purpose**: Use cases and business workflows
- Application services (orchestration)
- DTOs (Data Transfer Objects)
- Service interfaces
- Application-specific business rules

**Rules**:
- ✅ Can reference Domain layer
- ❌ Cannot reference Infrastructure or Presentation
- ✅ Defines interfaces for Infrastructure to implement
- ✅ Maps between Domain entities and DTOs

**Example**:
```csharp
// Application/Services/PersonService.cs
public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository; // Domain interface
    
    public async Task<PersonDto> CreatePersonAsync(CreatePersonRequest request)
    {
        var person = new Person { /* map from request */ };
        var created = await _repository.CreateAsync(person);
        return MapToDto(created); // Return DTO, not entity
    }
}
```

### 3. Infrastructure Layer
**Location**: `Modules/{ModuleName}/Infrastructure/`

**Purpose**: External concerns and implementations
- Repository implementations (database, file system)
- External service integrations
- Framework-specific code

**Rules**:
- ✅ Implements interfaces defined in Domain/Application
- ✅ Can reference Domain and Application
- ❌ Should not contain business logic
- ✅ Can use external libraries and frameworks

**Example**:
```csharp
// Infrastructure/Repositories/FilePersonRepository.cs
public class FilePersonRepository : IPersonRepository // Domain interface
{
    public async Task<Person> CreateAsync(Person person)
    {
        // File system specific implementation
        await File.WriteAllTextAsync(path, json);
        return person;
    }
}
```

### 4. Presentation Layer (Outermost)
**Location**: `Modules/{ModuleName}/Presentation/`

**Purpose**: HTTP/API concerns
- Minimal API endpoints
- Request/response models
- HTTP-specific logic
- API routing

**Rules**:
- ✅ References Application layer (calls services)
- ❌ Should NOT reference Infrastructure directly
- ✅ Handles HTTP concerns only
- ✅ Maps HTTP requests to service calls

**Example**:
```csharp
// Presentation/Endpoints/PersonEndpoints.cs
public static IEndpointRouteBuilder MapPersonEndpoints(this IEndpointRouteBuilder endpoints)
{
    endpoints.MapPost("/api/people", async (CreatePersonRequest request, IPersonService service) =>
    {
        var created = await service.CreatePersonAsync(request);
        return Results.Created($"/api/people/{created.Id}", created);
    });
}
```

## Module Structure

Each module is self-contained and follows the same pattern:

```
Modules/{ModuleName}/
├── Domain/
│   ├── Entities/           # Core business objects
│   └── Repositories/       # Repository interfaces
├── Application/
│   ├── Services/           # Application services & interfaces
│   └── DTOs/               # Data Transfer Objects
├── Infrastructure/
│   └── Repositories/       # Repository implementations
├── Presentation/
│   └── Endpoints/          # API endpoints
└── {ModuleName}Module.cs   # Module registration
```

## Dependency Flow

```
┌─────────────────────────────────────┐
│       Presentation Layer            │
│   (Endpoints, HTTP handling)        │
└────────────┬────────────────────────┘
             │ References
             ▼
┌─────────────────────────────────────┐
│      Application Layer              │
│  (Services, Use Cases, DTOs)        │
└────────────┬────────────────────────┘
             │ References
             ▼
┌─────────────────────────────────────┐
│        Domain Layer                 │
│  (Entities, Interfaces)             │
└─────────────────────────────────────┘
             ▲
             │ Implements
┌────────────┴────────────────────────┐
│     Infrastructure Layer            │
│  (Repositories, External Services)  │
└─────────────────────────────────────┘
```

## Why This Architecture?

### 1. **Testability**
- Each layer can be tested independently
- Mock interfaces in Application layer
- No need for actual database/file system in tests

### 2. **Flexibility**
- Swap Infrastructure implementations without changing business logic
- Change from file storage to SQL database by replacing repository
- Add caching, validation, etc. without touching core logic

### 3. **Maintainability**
- Clear separation of concerns
- Easy to find where to make changes
- Reduced coupling between layers

### 4. **Microservices Readiness**
- Each module is already isolated
- Clear boundaries between modules
- Easy to extract a module to its own service

## Module Communication

### Current (Monolith)
Modules are loosely coupled:
- **By ID Reference**: People store `addressIds`, not Address objects
- **No Direct Database Sharing**: Each module has its own JSON file
- **Through Contracts**: `Shared/Contracts/DomainEvents.cs` for future event-driven communication

### Future (Microservices)
When extracting to microservices:

1. **Replace In-Memory Calls with HTTP**
```csharp
// Before (Monolith)
var address = await _addressService.GetAddressAsync(addressId);

// After (Microservices)
var address = await _httpClient.GetFromJsonAsync<AddressDto>($"http://address-service/api/addresses/{addressId}");
```

2. **Use Message Bus for Events**
```csharp
// Publish event when person is created
await _messageBus.PublishAsync(new PersonCreatedEvent(person.Id, person.Email));

// Address service subscribes to this event if needed
```

3. **Separate Databases**
```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│   People     │     │  Addresses   │     │  Businesses  │
│  Service     │────▶│  Service     │◀────│  Service     │
│              │ HTTP│              │ HTTP│              │
│ SQL Database │     │ SQL Database │     │ SQL Database │
└──────────────┘     └──────────────┘     └──────────────┘
```

## Microservices Migration Strategy

### Phase 1: Monolith (Current)
- All modules in one process
- Shared middleware
- File-based storage

### Phase 2: Modular Monolith (Current State)
✅ Clear module boundaries
✅ Separate data storage per module
✅ Loose coupling via IDs and contracts

### Phase 3: Distributed Monolith (Preparation)
- Add API Gateway
- Implement circuit breakers
- Add distributed tracing
- Implement health checks

### Phase 4: Extract First Service
1. Choose a module (e.g., Addresses)
2. Create new project with just that module
3. Deploy independently
4. Update other modules to call via HTTP

### Phase 5: Full Microservices
- Each module becomes a service
- Message bus for async communication
- API Gateway for routing
- Separate databases

## Best Practices

### DO ✅
- Keep modules independent
- Use interfaces for all dependencies
- Return DTOs from services, not entities
- Store only IDs as cross-module references
- Use the Application layer for orchestration
- Keep Domain layer pure (no dependencies)

### DON'T ❌
- Don't share databases between modules
- Don't reference other module's entities directly
- Don't put business logic in Presentation layer
- Don't skip the Application layer
- Don't create circular dependencies between modules
- Don't expose Domain entities via API

## Adding New Modules

### Checklist
- [ ] Create Domain entities and repository interfaces
- [ ] Create Application services and DTOs
- [ ] Implement Infrastructure repositories
- [ ] Create Presentation endpoints
- [ ] Create Module registration class
- [ ] Register in `Program.cs`
- [ ] Add to README documentation
- [ ] Create API tests

### Template
```csharp
// {Module}Module.cs
public static class {Module}Module
{
    public static IServiceCollection Add{Module}Module(this IServiceCollection services)
    {
        services.AddScoped<I{Entity}Repository, File{Entity}Repository>();
        services.AddScoped<I{Entity}Service, {Entity}Service>();
        return services;
    }

    public static IEndpointRouteBuilder Map{Module}Module(this IEndpointRouteBuilder endpoints)
    {
        endpoints.Map{Entity}Endpoints();
        return endpoints;
    }
}
```

## Summary

This architecture provides:
- **Clear separation** between layers
- **Testable** code at every level
- **Flexible** implementations
- **Microservices-ready** structure
- **Maintainable** codebase

The Application layer is **essential** for:
- Orchestrating complex workflows
- Mapping between entities and DTOs
- Keeping presentation and domain independent
- Preparing for microservices migration
