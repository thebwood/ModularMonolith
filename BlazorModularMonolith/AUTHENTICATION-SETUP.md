# Authentication & Authorization Setup

## Overview
This project now includes JWT bearer token authentication for both the API and Blazor Web App.

## API Authentication Module

### Structure
The Authentication module follows the modular monolith pattern:

```
Modules/Authentication/
├── Domain/
│   ├── Entities/
│   │   └── User.cs
│   └── Repositories/
│       └── IUserRepository.cs
├── Application/
│   ├── DTOs/
│   │   ├── LoginRequest.cs
│   │   ├── LoginResponse.cs
│   │   └── RegisterRequest.cs
│   └── Services/
│       ├── IAuthenticationService.cs
│       └── AuthenticationService.cs
├── Infrastructure/
│   └── Repositories/
│       └── FileUserRepository.cs
├── Presentation/
│   └── Endpoints/
│       └── AuthenticationEndpoints.cs
└── AuthenticationModule.cs
```

### Default Users
Two default users are created automatically:
- **Admin**: username `admin`, password `Admin123!` (roles: Admin, User)
- **User**: username `user`, password `User123!` (role: User)

### API Endpoints
- `POST /api/v1/auth/login` - Login and receive JWT token
- `POST /api/v1/auth/register` - Register new user
- `GET /api/v1/auth/validate` - Validate JWT token

### JWT Configuration
JWT settings are configured in `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!123",
    "Issuer": "BlazorModularMonolith.Api",
    "Audience": "BlazorModularMonolith.Web",
    "ExpirationHours": 8
  }
}
```

**⚠️ Important**: Change the `SecretKey` in production!

### Protected Endpoints
All endpoints in the following modules are now protected and require authentication:
- `/api/v1/addresses/*` - Address management endpoints
- `/api/v1/people/*` - People management endpoints
- `/api/v1/businesses/*` - Business management endpoints

Authentication endpoints (`/api/v1/auth/*`) are public.

### CORS Configuration
CORS is enabled to allow the Blazor Web App to make authenticated requests:
- Allowed origins: `https://localhost:7189`, `http://localhost:5189`
- Allows credentials (required for bearer tokens)

## Blazor Web App Authentication

### Components
1. **Models/**
   - `LoginModel.cs` - Login form model
   - `UserInfo.cs` - User information including token

2. **Services/**
   - `IAuthService.cs` - Authentication service interface
   - `AuthService.cs` - Handles login, logout, and token management

3. **Handlers/**
   - `AuthenticationDelegatingHandler.cs` - Automatically adds bearer token to API requests

4. **Components/**
   - `Pages/Login.razor` - Login page
   - `Shared/LoginDisplay.razor` - Shows login status in navigation
   - `Shared/AuthorizeView.razor` - Component to protect pages

### Token Storage
JWT tokens are stored in browser localStorage:
- Token is automatically added to all API requests
- Token expiration is checked on each request
- Users are redirected to login if token expires

### Protected Pages
The following pages require authentication:
- `/people` - People management
- `/businesses` - Business management

### Usage

#### Login Flow
1. User navigates to `/login`
2. Enters credentials
3. On successful login:
   - JWT token is received from API
   - Token stored in localStorage
   - User redirected to home page

#### Automatic Token Attachment
The `AuthenticationDelegatingHandler` automatically:
- Retrieves token from localStorage
- Adds it as `Authorization: Bearer <token>` header
- Applies to all API service calls (AddressApiService, PersonApiService, BusinessApiService)

#### Logout Flow
1. User clicks "Logout" in navigation
2. Token removed from localStorage
3. User redirected to login page

## Testing Authentication

### 1. Start both projects
```bash
./run-all.ps1
```

### 2. Test API Authentication with curl
```bash
# Login
curl -X POST https://localhost:7188/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'

# Use token in subsequent requests
curl -X GET https://localhost:7188/api/v1/people \
  -H "Authorization: Bearer <your-token-here>"
```

### 3. Test Web App
1. Navigate to `https://localhost:7189`
2. Try accessing `/people` or `/businesses` - you'll be redirected to `/login`
3. Login with default credentials
4. Now you can access protected pages

## Security Considerations

### For Development
- Default users with known passwords (change in production)
- Simple password hashing with SHA256 (use better hashing in production)
- Secret key in appsettings.json (use Azure Key Vault or environment variables in production)

### For Production
1. **Change Secret Key**: Use a cryptographically secure random key
2. **Use Better Password Hashing**: Implement BCrypt or Argon2
3. **Store Secrets Securely**: Use Azure Key Vault, AWS Secrets Manager, or environment variables
4. **Enable HTTPS**: Ensure all communication is encrypted
5. **Add Refresh Tokens**: Implement token refresh mechanism
6. **Add Rate Limiting**: Protect against brute force attacks
7. **Implement 2FA**: Add two-factor authentication
8. **Audit Logging**: Log authentication attempts

## Extending Authentication

### Add Custom Claims
Edit `AuthenticationService.GenerateJwtToken()` to add custom claims:
```csharp
claims.Add(new Claim("CustomClaim", "CustomValue"));
```

### Role-Based Authorization
Protect specific endpoints with roles:
```csharp
group.MapDelete("/{id:guid}", DeletePerson)
    .RequireAuthorization(policy => policy.RequireRole("Admin"));
```

### Policy-Based Authorization
Create custom authorization policies in `Program.cs`:
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));
});
```

## Troubleshooting

### API returns 401 Unauthorized
- Ensure you're logged in and have a valid token
- Check token hasn't expired (default: 8 hours)
- Verify token is being sent in Authorization header

### Login fails
- Check API is running on correct port (default: 7188)
- Verify credentials match default users
- Check browser console for errors

### Token not being sent
- Verify `AuthenticationDelegatingHandler` is registered
- Check localStorage contains token
- Ensure handler is added to HttpClient configuration
