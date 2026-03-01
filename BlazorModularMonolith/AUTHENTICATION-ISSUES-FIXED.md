# Authentication Issues Fixed - Updated Implementation

## Problems Identified

1. **Token not being sent with API requests** - Scoped TokenProvider wasn't working correctly with Blazor Server circuits
2. **No Register UI** - Only Login page existed
3. **Token persistence issues** - Token wasn't persisting across component navigations

## Solutions Implemented

### 1. Fixed TokenProvider with Circuit-Aware Storage

**Problem**: The scoped TokenProvider was creating a new instance per scope, losing the token between HTTP requests.

**Solution**: Changed TokenProvider to:
- Use `IHttpContextAccessor` to identify the current circuit/connection
- Store tokens in a static `ConcurrentDictionary` keyed by circuit ID
- Changed from Scoped to Singleton lifetime
- Each user's circuit gets its own token storage

**Files Changed**:
- `TokenProvider.cs` - Now uses ConcurrentDictionary with circuit IDs
- `Program.cs` - Added `AddHttpContextAccessor()` and changed TokenProvider to Singleton

### 2. Fixed AuthenticationDelegatingHandler Scope Resolution

**Problem**: The handler was trying to use TokenProvider directly, but scopes weren't aligning correctly.

**Solution**: Changed handler to:
- Inject `IServiceProvider` instead of `ITokenProvider`
- Create a scope per request to resolve TokenProvider
- This ensures the correct circuit's token is retrieved

**Files Changed**:
- `AuthenticationDelegatingHandler.cs` - Now creates scope per request

### 3. Added Register Page

**New Features**:
- Full registration UI at `/register`
- Username, email, password, and confirm password fields
- Validation and error handling
- Automatic login after successful registration
- Link from Login page to Register page

**Files Created**:
- `Components/Pages/Register.razor` - Registration page

**Files Updated**:
- `IAuthService.cs` - Added `RegisterAsync()` method
- `AuthService.cs` - Implemented `RegisterAsync()` to call API
- `Login.razor` - Added link to register page

## How the Authentication Flow Works Now

### Registration Flow
```
1. User navigates to /register
2. Enters username, email, and password
3. Form submitted → AuthService.RegisterAsync()
4. Calls /api/v1/auth/register endpoint
5. API creates user and returns JWT token
6. Token stored in:
   - Browser localStorage (for persistence)
   - TokenProvider ConcurrentDictionary (for API requests)
7. User automatically logged in and redirected to home
```

### Login Flow
```
1. User navigates to /login
2. Enters credentials
3. Form submitted → AuthService.LoginAsync()
4. Calls /api/v1/auth/login endpoint
5. API validates credentials and returns JWT token
6. Token stored in:
   - Browser localStorage (for persistence)
   - TokenProvider ConcurrentDictionary (for API requests)
7. User redirected to home
```

### API Request Flow (Fixed!)
```
1. Component calls API service (e.g., PersonApiService.GetAllAsync())
2. HttpClient request intercepted by AuthenticationDelegatingHandler
3. Handler creates new scope from IServiceProvider
4. Resolves ITokenProvider from scope
5. TokenProvider uses HttpContext.Connection.Id to identify circuit
6. Retrieves token from ConcurrentDictionary[circuitId]
7. Adds "Authorization: Bearer <token>" header
8. Request sent to API
9. API validates token ✅
10. Data returned successfully ✅
```

### Session Persistence (After Page Refresh)
```
1. User refreshes page
2. Blazor component initializes
3. AuthorizeView or component calls AuthService.GetCurrentUserAsync()
4. Token retrieved from localStorage (JSInterop)
5. Token stored back in TokenProvider ConcurrentDictionary
6. User remains authenticated
7. Subsequent API requests work ✅
```

## Testing Steps

### 1. Clear Browser Data (Important!)
```
1. Open Developer Tools (F12)
2. Go to Application tab
3. Clear localStorage
4. Close and reopen browser
```

### 2. Test Registration
```bash
# Restart both projects
./run-all.ps1

# In browser:
1. Navigate to https://localhost:7189/register
2. Enter:
   - Username: testuser
   - Email: test@example.com
   - Password: Test123!
   - Confirm Password: Test123!
3. Click Register
4. Should see success message
5. Should redirect to home page
6. Should see "testuser" in navigation
```

### 3. Test Login
```bash
# In browser:
1. Click Logout
2. Click Login
3. Enter:
   - Username: admin
   - Password: Admin123!
4. Click Login
5. Should see "admin" in navigation
```

### 4. Test Protected Pages
```bash
# In browser:
1. Navigate to /people
2. Should load successfully (no "Failed to load" error) ✅
3. Try creating a new person ✅
4. Navigate to /businesses
5. Should load successfully ✅
6. Try creating a new business ✅
```

### 5. Test Token Persistence
```bash
# In browser:
1. Login as admin
2. Navigate to /people
3. Verify data loads
4. Press F5 (refresh page)
5. Should still be logged in ✅
6. Data should load again ✅
```

## Verification Checklist

- [ ] Can register new user at `/register`
- [ ] Can login with existing credentials
- [ ] Can login with newly registered user
- [ ] Username shows in navigation after login
- [ ] `/people` page loads data successfully
- [ ] Can create new person successfully
- [ ] `/businesses` page loads data successfully
- [ ] Can create new business successfully
- [ ] Token persists after page refresh
- [ ] Still authenticated after refresh
- [ ] Logout clears token and redirects to login

## Troubleshooting

### Issue: Still getting "Failed to load" errors

**Check**:
1. API is running (https://localhost:7188)
2. Browser console for detailed errors (F12)
3. Network tab shows Authorization header with Bearer token
4. API returns 200, not 401

**Fix**:
- Clear browser localStorage
- Logout and login again
- Check browser console for JavaScript errors

### Issue: "Failed to create person/business"

**This was caused by**: Token not being sent to API

**Now fixed by**: Circuit-aware TokenProvider and proper scope resolution in AuthenticationDelegatingHandler

### Issue: Token not persisting after refresh

**Check**:
1. localStorage has "authToken" and "userData" keys
2. TokenProvider is registered as Singleton
3. HttpContextAccessor is registered

## Architecture Improvements

### Before (Broken):
- ❌ Scoped TokenProvider per request
- ❌ Token lost between requests
- ❌ Handler couldn't access correct scope

### After (Fixed):
- ✅ Singleton TokenProvider with circuit-based storage
- ✅ Token persists for entire user session
- ✅ Handler creates scope to access TokenProvider
- ✅ HttpContextAccessor identifies user's circuit
- ✅ ConcurrentDictionary thread-safe storage

## Key Files

- `Services/ITokenProvider.cs` - Token management interface
- `Services/TokenProvider.cs` - Circuit-aware token storage (Singleton)
- `Services/AuthService.cs` - Login/Register/Token management
- `Handlers/AuthenticationDelegatingHandler.cs` - Adds Bearer token to requests
- `Components/Pages/Login.razor` - Login UI
- `Components/Pages/Register.razor` - Registration UI (NEW)
- `Program.cs` - DI registration

## Important Notes

- TokenProvider is now **Singleton** (not Scoped)
- Tokens are stored per circuit ID (connection ID)
- Concurrent users each get their own token storage
- Thread-safe with ConcurrentDictionary
- Automatic cleanup not implemented (tokens remain until app restart)
- For production: Implement token cleanup on circuit disposal
