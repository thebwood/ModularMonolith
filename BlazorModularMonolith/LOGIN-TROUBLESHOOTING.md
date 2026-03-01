# Login Issues Troubleshooting

## Problem
Login with `admin` / `Admin123!` returns "Invalid username or password"

## Verification Steps Completed

### 1. Checked users.json File ✅
```
Location: BlazorModularMonolith.Api/Data/users.json
Status: File exists
Admin user: Present with correct hash
```

### 2. Verified Password Hash ✅
```
Expected: PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=
Actual:   PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=
Match: YES ✅
```

### 3. Verified Hashing Logic ✅
- FileUserRepository uses SHA256
- AuthenticationService uses SHA256
- Both use same encoding and Base64 conversion
- VerifyPassword logic is correct

## Enhanced Debugging

### Changes Made

#### 1. Updated AuthService.cs
Added detailed logging to see actual HTTP response:
```csharp
if (!response.IsSuccessStatusCode)
{
    var errorContent = await response.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine($"Login failed with status {response.StatusCode}: {errorContent}");
    return null;
}
```

#### 2. Updated Login.razor
Added logging to track login attempts:
```csharp
System.Diagnostics.Debug.WriteLine($"Attempting login with username: '{_loginModel.Username}'");
```

## How to Debug

### Step 1: Open Output Window
1. In Visual Studio, go to **View** → **Output**
2. Select "Debug" from the dropdown
3. Keep this window visible

### Step 2: Start Both Projects
```bash
./run-all.ps1
```

### Step 3: Open Browser Console
1. Press **F12** in the browser
2. Go to **Console** tab
3. Keep it open

### Step 4: Attempt Login
1. Navigate to `https://localhost:7189/login`
2. Enter: `admin` / `Admin123!`
3. Click Login
4. **Check both Output window and Browser console for error messages**

## Common Issues & Solutions

### Issue 1: API Not Running
**Symptom**: "Unable to connect to remote server"

**Solution**:
```bash
# Make sure both projects are running
./run-all.ps1

# Verify API is running
# Open: https://localhost:7188
# Should see: {"Message": "Address Management API..."}
```

### Issue 2: CORS Errors
**Symptom**: CORS policy error in browser console

**Check**: BlazorModularMonolith.Api/Program.cs
```csharp
app.UseCors("AllowBlazorApp");
```

**Verify origins**:
```csharp
policy.WithOrigins("https://localhost:7189", "http://localhost:5189")
```

### Issue 3: Token Not Being Sent
**Symptom**: Login appears successful but subsequent requests fail

**Check**: TokenProvider is storing token
- Look for Debug output: "Token stored in TokenProvider"
- Check browser localStorage (F12 → Application → localStorage)

### Issue 4: Username Case Sensitivity
**Symptom**: Works sometimes, not other times

**Check**: AuthenticationService uses case-insensitive comparison:
```csharp
return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
```

### Issue 5: Password Whitespace
**Symptom**: Correct credentials fail

**Test**: Remove any leading/trailing spaces
```csharp
username = loginModel.Username?.Trim() ?? "";
password = loginModel.Password?.Trim() ?? "";
```

## Testing Checklist

### Pre-Login Verification
- [ ] Both API and Web projects running
- [ ] API accessible at `https://localhost:7188`
- [ ] Web accessible at `https://localhost:7189`
- [ ] users.json file exists with correct data
- [ ] Output window open in Visual Studio
- [ ] Browser console open (F12)

### During Login
- [ ] Enter username: `admin` (all lowercase)
- [ ] Enter password: `Admin123!` (exact match)
- [ ] No extra spaces
- [ ] Click Login
- [ ] Watch Output window for Debug messages
- [ ] Watch Browser console for errors

### Expected Debug Output

**Success Path**:
```
Attempting login with username: 'admin'
Login successful!
Token stored in TokenProvider
```

**Failure Path**:
```
Attempting login with username: 'admin'
Login failed with status 401: {"message": "..."}
OR
Login exception: [error details]
```

## Manual API Test

### Using PowerShell (with API running)
```powershell
# Test login endpoint directly
$body = @{
    username = "admin"
    password = "Admin123!"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7188/api/v1/auth/login" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"
```

**Expected Result**: Token and user info

### Using Browser (Swagger/Scalar)
1. Navigate to `https://localhost:7188/scalar/v1`
2. Find `POST /api/v1/auth/login`
3. Click "Try it out"
4. Enter:
```json
{
  "username": "admin",
  "password": "Admin123!"
}
```
5. Execute
6. Should return 200 OK with token

## Files to Check

### 1. users.json
```bash
BlazorModularMonolith.Api/Data/users.json
```
Verify admin user exists and is active.

### 2. API appsettings.json
```bash
BlazorModularMonolith.Api/appsettings.json
```
Check JwtSettings are configured.

### 3. Web appsettings.json
```bash
BlazorModularMonolith.Web/BlazorModularMonolith.Web/appsettings.json
```
Check ApiSettings:BaseUrl points to correct API URL.

## Nuclear Option: Reset Everything

If nothing works, try this:

### 1. Delete users.json
```powershell
Remove-Item "BlazorModularMonolith.Api\Data\users.json"
```

### 2. Clear Browser Data
- Press F12
- Application tab → Clear storage
- Clear all localStorage, sessionStorage, cookies

### 3. Restart Both Projects
```bash
./run-all.ps1
```

### 4. Verify users.json Recreated
The file should be automatically recreated with default users.

## Still Not Working?

### Collect Debug Information

1. **Output Window Content** (View → Output → Debug)
2. **Browser Console Errors** (F12 → Console)
3. **Network Tab** (F12 → Network → Filter by "login")
   - Check request payload
   - Check response status and body
4. **users.json Content**
```powershell
Get-Content "BlazorModularMonolith.Api\Data\users.json"
```

### Create Issue Report

Include:
- Output window logs
- Browser console errors
- Network request/response details
- users.json content (hash is safe to share, it's not reversible)
- Steps you followed

## Quick Reference

| Credential | Value |
|------------|-------|
| Admin Username | `admin` |
| Admin Password | `Admin123!` |
| User Username | `user` |
| User Password | `User123!` |

**Important**: Passwords are case-sensitive! Usernames are case-insensitive.
