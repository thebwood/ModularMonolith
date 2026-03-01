# Blazor Form Submission Fix

## Problem

When trying to login or register, you got this error:
```
The POST request does not specify which form is being submitted. 
To fix this, ensure <form> elements have a @formname attribute with any unique value, 
or pass a FormName parameter if using <EditForm>.
```

## Root Cause

In .NET 8+ with Blazor Server, forms require explicit identification to prevent cross-site request forgery (CSRF) attacks and to properly handle multiple forms on the same page. This is a security feature.

## Solution Applied

### 1. Added `FormName` to EditForm Components

**Login.razor**:
```razor
<EditForm Model="@_loginModel" OnValidSubmit="@HandleLogin" FormName="loginForm">
```

**Register.razor**:
```razor
<EditForm Model="@_registerModel" OnValidSubmit="@HandleRegister" FormName="registerForm">
```

### 2. Added `[SupplyParameterFromForm]` Attribute

This attribute tells Blazor to bind the form data to the model from the form submission.

**Login.razor** @code section:
```csharp
[SupplyParameterFromForm]
private LoginModel _loginModel { get; set; } = new();
```

**Register.razor** @code section:
```csharp
[SupplyParameterFromForm]
private RegisterModel _registerModel { get; set; } = new();
```

## What Changed

| Before | After |
|--------|-------|
| `<EditForm Model="@_loginModel" OnValidSubmit="@HandleLogin">` | `<EditForm Model="@_loginModel" OnValidSubmit="@HandleLogin" FormName="loginForm">` |
| `private LoginModel _loginModel = new();` | `[SupplyParameterFromForm]`<br/>`private LoginModel _loginModel { get; set; } = new();` |

## Why This Matters

1. **Security**: Prevents CSRF attacks by ensuring form submissions are legitimate
2. **Multi-Form Support**: Allows multiple forms on the same page to be distinguished
3. **Blazor Server Requirements**: Required for proper server-side form handling in .NET 8+
4. **Form State Management**: Properly manages form state across server round-trips

## Testing

1. **Restart the application**: `./run-all.ps1`
2. **Navigate to Login**: `https://localhost:7189/login`
3. **Enter credentials**:
   - Username: `admin`
   - Password: `Admin123!`
4. **Submit form** ✅ Should work without error
5. **Navigate to Register**: `https://localhost:7189/register`
6. **Fill registration form** and submit ✅ Should work without error

## Additional Notes

### FormName Best Practices

- **Unique Names**: Each form on a page should have a unique FormName
- **Descriptive**: Use descriptive names like "loginForm", "registerForm", "profileForm"
- **Required**: Always include FormName in .NET 8+ Blazor Server apps

### SupplyParameterFromForm

- **Purpose**: Binds form data to component properties
- **Usage**: Applied to properties that receive form data
- **Automatic Binding**: Blazor automatically populates the property on form submission

### When You Need This

You need `FormName` and `[SupplyParameterFromForm]` when:
- Using `<EditForm>` in Blazor Server (.NET 8+)
- Submitting forms via POST
- Using server-side rendering with forms

You DON'T need this when:
- Using Blazor WebAssembly only (client-side)
- Using JavaScript form submissions
- Not using `<EditForm>` component

## Files Modified

- `BlazorModularMonolith.Web/BlazorModularMonolith.Web/Components/Pages/Login.razor`
- `BlazorModularMonolith.Web/BlazorModularMonolith.Web/Components/Pages/Register.razor`

## Related Documentation

- [ASP.NET Core Blazor forms overview](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms/)
- [Enhanced form handling in Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms/enhanced-handling)

## Verification Checklist

- [x] Added `FormName="loginForm"` to Login.razor EditForm
- [x] Added `FormName="registerForm"` to Register.razor EditForm
- [x] Added `[SupplyParameterFromForm]` to Login.razor model property
- [x] Added `[SupplyParameterFromForm]` to Register.razor model property
- [x] Changed model field to property (with getter/setter)
- [x] Build successful
- [ ] Login form works without error
- [ ] Register form works without error
