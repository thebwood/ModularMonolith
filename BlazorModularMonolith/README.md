# Address Management API - Modular Monolith

A modular monolith API built with .NET 10 Minimal APIs for managing addresses, people, and businesses with proper separation of concerns and microservices-ready architecture.

## Architecture

This project follows **Clean Architecture** principles with a **Modular Monolith** design, structured for easy migration to microservices.

### Module Structure (Clean Architecture Layers)

```
Modules/
├── Addresses/
│   ├── Domain/                # Domain Layer
│   │   ├── Entities/         # Domain entities (Address, AddressType)
│   │   └── Repositories/     # Repository interfaces
│   ├── Application/          # Application Layer
│   │   ├── Services/         # Application services & interfaces
│   │   └── DTOs/             # Data Transfer Objects
│   ├── Infrastructure/       # Infrastructure Layer
│   │   └── Repositories/     # File-based repository implementations
│   ├── Presentation/         # Presentation Layer
│   │   └── Endpoints/        # Minimal API endpoints
│   └── AddressModule.cs      # Module registration & wiring
│
├── People/
│   ├── Domain/               # Person entity, interfaces
│   ├── Application/          # PersonService, DTOs
│   ├── Infrastructure/       # File storage
│   ├── Presentation/         # API endpoints
│   └── PeopleModule.cs
│
└── Businesses/
    ├── Domain/               # Business entity, interfaces
    ├── Application/          # BusinessService, DTOs
    ├── Infrastructure/       # File storage
    ├── Presentation/         # API endpoints
    └── BusinessesModule.cs
```

### Shared Components

```
Shared/
├── Middleware/               # Cross-cutting concerns
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Extensions/
│   └── MiddlewareExtensions.cs
└── Contracts/                # Inter-module contracts
    └── DomainEvents.cs       # Events for module communication
```

## Why the Application Layer?

**Yes, the Application layer is essential** for Clean Architecture and microservices readiness:

1. **Separation of Concerns**: Domain logic (entities) stays separate from application logic (use cases)
2. **Testability**: Application services can be unit tested independently
3. **DTOs**: Prevents exposing domain entities directly via API
4. **Microservices Ready**: Each module's application layer can become a microservice
5. **Business Rules**: Complex orchestration logic lives here, not in controllers/endpoints

## Features

### ✅ Modular Architecture
- **Self-contained modules**: Each module has its own domain, data, and endpoints
- **Loose coupling**: Modules communicate via contracts/events
- **Independent deployment ready**: Each module can be extracted to a microservice

### ✅ Clean Architecture Benefits
- **Domain Layer**: Pure business entities with no dependencies
- **Application Layer**: Use cases and business logic orchestration
- **Infrastructure Layer**: Framework-specific implementations
- **Presentation Layer**: HTTP/API concerns only

### ✅ Technical Features
- **API Versioning**: URL path versioning (`/api/v1/...`) for future-proof APIs
- **Minimal APIs**: Lightweight endpoint definitions
- **File-based storage**: JSON storage - each module has its own data file
- **Comprehensive logging**: Middleware for request/response logging
- **Global exception handling**: Standardized error responses
- **OpenAPI/Swagger**: Interactive Scalar documentation
- **Thread-safe operations**: Semaphore-based file locking

## API Endpoints

### Addresses Module (`/api/v1/addresses`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/addresses` | Get all addresses |
| GET | `/api/v1/addresses/{id}` | Get address by ID |
| GET | `/api/v1/addresses/owner/{ownerId}` | Get addresses by owner ID |
| GET | `/api/v1/addresses/type/{type}` | Get addresses by type (Person, Business, Other) |
| POST | `/api/v1/addresses` | Create a new address |
| PUT | `/api/v1/addresses/{id}` | Update an existing address |
| DELETE | `/api/v1/addresses/{id}` | Delete an address |

### People Module (`/api/v1/people`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/people` | Get all people |
| GET | `/api/v1/people/{id}` | Get person by ID |
| GET | `/api/v1/people/email/{email}` | Get person by email |
| POST | `/api/v1/people` | Create a new person |
| PUT | `/api/v1/people/{id}` | Update an existing person |
| DELETE | `/api/v1/people/{id}` | Delete a person |
| POST | `/api/v1/people/{id}/addresses/{addressId}` | Add address to person |
| DELETE | `/api/v1/people/{id}/addresses/{addressId}` | Remove address from person |

### Businesses Module (`/api/v1/businesses`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/businesses` | Get all businesses |
| GET | `/api/v1/businesses/{id}` | Get business by ID |
| GET | `/api/v1/businesses/taxid/{taxId}` | Get business by tax ID |
| GET | `/api/v1/businesses/type/{type}` | Get businesses by type |
| POST | `/api/v1/businesses` | Create a new business |
| PUT | `/api/v1/businesses/{id}` | Update an existing business |
| DELETE | `/api/v1/businesses/{id}` | Delete a business |
| POST | `/api/v1/businesses/{id}/addresses/{addressId}` | Add address to business |
| DELETE | `/api/v1/businesses/{id}/addresses/{addressId}` | Remove address from business |

## API Versioning

This API implements **URL Path Versioning**:
- **Current version**: `v1`
- **Version format**: `/api/v{version}/{resource}`
- **Default version**: `1.0` (assumed if not specified)

### Version Information in Headers
Each response includes version information in headers:
- `api-supported-versions`: Lists all supported versions
- `api-deprecated-versions`: Lists deprecated versions (if any)

### Future Versions
To add a new version:
1. Create new endpoint methods or modify existing ones
2. Update the version set in `Program.cs`
3. Map endpoints with the new version
4. Maintain backward compatibility with v1 when possible

Example for v2:
```csharp
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();
```

## Getting Started

### Prerequisites

- .NET 10 SDK

### Run the Application

```bash
cd BlazorModularMonolith.Api
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7188`
- HTTP: `http://localhost:5137`

### Access API Documentation

When running in Development mode, the browser will automatically open to the **Scalar API Documentation** interface at:
- `https://localhost:7188/scalar/v1`

You can also access:
- **Scalar UI**: `/scalar/v1` - Modern interactive API documentation
- **OpenAPI Spec**: `/openapi/v1.json` - Raw OpenAPI/Swagger specification

The Scalar UI provides:
- Interactive API testing
- Request/response examples
- Schema documentation
- Beautiful purple-themed interface

## Data Storage

Each module uses its own JSON flat file for storage:
- `Data/addresses.json` - Address data
- `Data/people.json` - People data
- `Data/businesses.json` - Business data

This separation makes it easy to migrate each module to its own database later.

### Storage Configuration

Configure the data directory in `appsettings.json`:

```json
{
  "Storage": {
    "DataDirectory": "Data"
  }
}
```

## Microservices Migration Path

The architecture is designed for easy extraction to microservices:

### Current State (Modular Monolith)
- All modules in one process
- Shared middleware and configuration
- In-memory/file-based communication

### Migration Steps
1. **Extract a module** (e.g., Addresses)
   - Copy the module folder to a new project
   - Add its own Program.cs and startup
   - Deploy independently

2. **Change communication**
   - Replace in-memory calls with HTTP/gRPC
   - Use domain events via message bus (RabbitMQ, Azure Service Bus)

3. **Separate data stores**
   - Already separated by file!
   - Migrate to dedicated databases (SQL, Cosmos, etc.)

4. **Deploy independently**
   - Each module becomes its own service
   - Use API Gateway for routing

### Module Boundaries
- ✅ **No direct database sharing** (each module has own file)
- ✅ **No cross-module entity references** (use IDs only)
- ✅ **Communication through contracts** (events in Shared/Contracts)
- ✅ **Independent deployment** (each module can run standalone)

## Adding New Modules

To add a new module (e.g., Contacts):

1. Create the folder structure:
```
Modules/Contacts/
├── Domain/
│   ├── Entities/
│   └── Repositories/
├── Application/
│   ├── Services/
│   └── DTOs/
├── Infrastructure/
│   └── Repositories/
├── Presentation/
│   └── Endpoints/
└── ContactsModule.cs
```

2. Register in `Program.cs`:
```csharp
builder.Services.AddContactsModule();
app.MapContactsModule();
```

## Middleware Pipeline

1. **Request Logging** - Logs all requests with timing
2. **Exception Handling** - Catches and formats errors
3. **HTTPS Redirection** - Security

## Project Benefits

- **Modularity**: Each module is independent and testable
- **Scalability**: Easy horizontal scaling or microservices migration
- **Maintainability**: Clear boundaries and responsibilities
- **Testability**: Each layer can be tested in isolation
- **Flexibility**: Swap implementations without changing business logic
- **Team Productivity**: Teams can work on different modules independently

## License

This project is open source and available under the MIT License.
