# Authentication Token Management Fix

## Problem

The authentication system was failing to attach JWT bearer tokens to API requests, causing "Failed to load people/businesses" errors.

### Root Cause

The `AuthenticationDelegatingHandler` was attempting to use `JSInterop` through `AuthService.GetCurrentUserAsync()` to retrieve the token from browser localStorage. However, the delegating handler executes in the server-side context during HttpClient requests, where JSInterop is not available. This caused tokens not to be attached to API requests.

## Solution

Implemented a scoped `TokenProvider` service to manage tokens in server-side memory during the user's session.

### Changes Made

1. **Created `ITokenProvider` and `TokenProvider`**
   - Simple scoped service that stores the token in memory
   - Accessible from both Blazor components (via JSInterop) and HttpClient handlers (server-side)

2. **Updated `AuthService`**
   - Injected `ITokenProvider`
   - Calls `SetToken()` after successful login
   - Calls `SetToken()` when retrieving user from localStorage
   - Calls `ClearToken()` on logout

3. **Updated `AuthenticationDelegatingHandler`**
   - Changed from using `IServiceProvider` + `AuthService` (which uses JSInterop)
   - Now directly injects `ITokenProvider` (server-side compatible)
   - Simply gets token from `TokenProvider.GetToken()`

4. **Updated `Program.cs`**
   - Registered `ITokenProvider` as scoped service

### Configuration Fix

Also fixed the data directory configuration path in `FileUserRepository`:
- Changed from `DataDirectory` to `Storage:DataDirectory` to match appsettings.json

## How It Works Now

### Login Flow
```
1. User enters credentials in Login.razor
2. AuthService.LoginAsync() calls API
3. API returns JWT token
4. AuthService stores token in:
   - Browser localStorage (via JSInterop)
   - TokenProvider (in-memory, server-side)
5. User is authenticated
```

### API Request Flow
```
1. Blazor component calls API service (e.g., PersonApiService)
2. HttpClient request goes through AuthenticationDelegatingHandler
3. Handler gets token from TokenProvider (server-side, no JSInterop needed)
4. Handler adds Authorization: Bearer <token> header
5. API receives authenticated request
6. Data returned successfully
```

### Session Persistence
```
1. User refreshes page
2. Blazor component initializes
3. AuthService.GetCurrentUserAsync() called
4. Token retrieved from localStorage (via JSInterop)
5. Token stored in TokenProvider (server-side)
6. Subsequent API requests have token available
```

## Testing

1. **Restart both projects** to apply fixes
2. **Login** with `admin` / `Admin123!`
3. **Access /people or /businesses** - data should load successfully
4. **Refresh page** - should stay authenticated
5. **Logout** - token cleared from both localStorage and TokenProvider

## Architecture Benefits

- **Separation of Concerns**: Token storage separated from authentication logic
- **Server-Side Compatible**: No JSInterop in HTTP request pipeline
- **Scoped Lifetime**: Token tied to user session
- **Dual Storage**: Browser localStorage for persistence + server memory for requests
- **Clean Code**: Simplified `AuthenticationDelegatingHandler`

## Files Modified

- `BlazorModularMonolith.Web/BlazorModularMonolith.Web/Services/ITokenProvider.cs` (new)
- `BlazorModularMonolith.Web/BlazorModularMonolith.Web/Services/TokenProvider.cs` (new)
- `BlazorModularMonolith.Web/BlazorModularMonolith.Web/Services/AuthService.cs` (updated)
- `BlazorModularMonolith.Web/BlazorModularMonolith.Web/Handlers/AuthenticationDelegatingHandler.cs` (updated)
- `BlazorModularMonolith.Web/BlazorModularMonolith.Web/Program.cs` (updated)
- `BlazorModularMonolith.Api/Modules/Authentication/Infrastructure/Repositories/FileUserRepository.cs` (updated)

## Important Notes

- The `TokenProvider` is scoped per user session (SignalR circuit in Server mode)
- Token is still persisted in localStorage for session resumption after refresh
- The fix maintains backward compatibility with the existing authentication flow
- No changes needed to API endpoints or authentication logic
