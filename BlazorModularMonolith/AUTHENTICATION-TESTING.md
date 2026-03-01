# Authentication Testing Guide

## Quick Start

### 1. Start Both Projects
```bash
./run-all.ps1
```

This will start:
- **API**: https://localhost:7188
- **Web App**: https://localhost:7189

### 2. Test the Web Application

1. **Open your browser** and navigate to: `https://localhost:7189`

2. **Try to access protected pages**:
   - Go to `/people` or `/businesses`
   - You'll be automatically redirected to `/login`

3. **Login with default credentials**:
   - **Admin User**: 
     - Username: `admin`
     - Password: `Admin123!`
   - **Regular User**: 
     - Username: `user`
     - Password: `User123!`

4. **After successful login**:
   - You'll be redirected to the home page
   - Your username will appear in the top navigation
   - You can now access all protected pages
   - Click "Logout" to end your session

### 3. Test the API Directly

#### Login via API
```bash
# PowerShell
$response = Invoke-RestMethod -Uri "https://localhost:7188/api/v1/auth/login" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"username":"admin","password":"Admin123!"}'

$token = $response.token
Write-Host "Token: $token"
```

#### Use Token to Access Protected Endpoints
```bash
# PowerShell
Invoke-RestMethod -Uri "https://localhost:7188/api/v1/people" `
  -Method GET `
  -Headers @{Authorization = "Bearer $token"}
```

#### Test Without Token (Should Fail)
```bash
# PowerShell
Invoke-RestMethod -Uri "https://localhost:7188/api/v1/people" -Method GET
# Expected: 401 Unauthorized
```

### 4. Test in Swagger/Scalar UI

1. Navigate to: `https://localhost:7188/scalar/v1`

2. **Login to get token**:
   - Find the `POST /api/v1/auth/login` endpoint
   - Click "Try it out"
   - Enter:
     ```json
     {
       "username": "admin",
       "password": "Admin123!"
     }
     ```
   - Click "Execute"
   - Copy the `token` from the response

3. **Use token for protected endpoints**:
   - You'll need to manually add the Authorization header
   - Format: `Bearer <your-token>`

## Default Test Users

| Username | Password | Roles | Description |
|----------|----------|-------|-------------|
| admin | Admin123! | Admin, User | Full access |
| user | User123! | User | Standard access |

## Verification Checklist

✅ **API Authentication Module**
- [ ] Can login at `/api/v1/auth/login`
- [ ] Can register at `/api/v1/auth/register`
- [ ] Protected endpoints return 401 without token
- [ ] Protected endpoints work with valid token

✅ **Web App Authentication**
- [ ] Login page accessible at `/login`
- [ ] Protected pages redirect to login
- [ ] After login, can access protected pages
- [ ] Username shows in navigation
- [ ] Logout button works
- [ ] After logout, redirected to login

✅ **Integration**
- [ ] Web app automatically includes token in API requests
- [ ] Token is persisted across page refreshes
- [ ] Expired tokens trigger re-login

## Security Notes

⚠️ **This is a development setup**. For production, see AUTHENTICATION-SETUP.md for security considerations.
