# Address Management System - Modular Monolith with Blazor

A complete full-stack application with .NET 10:
- **API**: Modular monolith with clean architecture
- **Web**: Blazor Web App with MVVM pattern

## Quick Start - Run Both Projects

### Option 1: Using PowerShell Script (Recommended)

**Run in separate terminal windows:**
```powershell
.\run-separate.ps1
```
This opens two terminal windows - one for API, one for Web App.

**Run in single terminal with job management:**
```powershell
.\run-all.ps1
```
This runs both in the background with colored output. Press Ctrl+C to stop both.

### Option 2: Using Visual Studio Code

1. Open the workspace in VS Code
2. Press `F5` or go to Run and Debug
3. Select **"API + Web: Launch Both"**
4. Both projects will start with debugging enabled

### Option 3: Manual Terminal Commands

**Terminal 1 - API:**
```bash
cd BlazorModularMonolith.Api
dotnet run
```

**Terminal 2 - Blazor Web:**
```bash
cd BlazorModularMonolith.Web/BlazorModularMonolith.Web
dotnet run
```

### Option 4: Using Visual Studio 2022

1. Open `BlazorModularMonolith.slnx`
2. Right-click solution → **Set Startup Projects**
3. Select **Multiple startup projects**
4. Set both projects to **Start**
5. Press F5

## Access the Applications

Once running, you can access:

| Application | URL | Description |
|-------------|-----|-------------|
| **API** | https://localhost:7188 | REST API backend |
| **API Docs** | https://localhost:7188/scalar/v1 | Interactive API documentation |
| **Web App** | https://localhost:7031 | Blazor Web UI |

## Project Structure

```
BlazorModularMonolith/
├── BlazorModularMonolith.Api/          # Backend API
│   ├── Modules/                        # Business modules
│   │   ├── Addresses/
│   │   ├── People/
│   │   └── Businesses/
│   └── Shared/                         # Cross-cutting concerns
│
├── BlazorModularMonolith.Web/          # Frontend Blazor App
│   ├── Models/                         # Data models
│   ├── Services/                       # API clients
│   ├── ViewModels/                     # MVVM ViewModels
│   ├── Components/Pages/               # Blazor pages
│   └── Middleware/                     # Web middleware
│
├── .vscode/                            # VS Code configuration
│   ├── launch.json                     # Debug configurations
│   └── tasks.json                      # Build tasks
│
├── run-all.ps1                         # Run both (single terminal)
└── run-separate.ps1                    # Run both (separate windows)
```

## Development Workflow

### Hot Reload (Development Mode)

Both projects support hot reload during development:

**API with hot reload:**
```bash
cd BlazorModularMonolith.Api
dotnet watch run
```

**Web App with hot reload:**
```bash
cd BlazorModularMonolith.Web/BlazorModularMonolith.Web
dotnet watch run
```

### Building

**Build all projects:**
```bash
dotnet build
```

**Build specific project:**
```bash
dotnet build BlazorModularMonolith.Api
dotnet build BlazorModularMonolith.Web/BlazorModularMonolith.Web
```

### Testing the Application

1. **Start both applications** using any method above
2. **API will auto-open** Scalar documentation at https://localhost:7188/scalar/v1
3. **Access Web App** at https://localhost:7031
4. **Navigate** to People or Businesses pages
5. **Perform CRUD operations** in the Web UI
6. **Data is stored** in JSON files in the `Data/` folder

## Architecture Overview

### API (Backend)
- **Pattern**: Clean Architecture + Modular Monolith
- **Layers**: Domain → Application → Infrastructure → Presentation
- **Storage**: JSON flat files (easy to migrate to database)
- **API Version**: v1 (URL path versioning)
- **Documentation**: Scalar UI (auto-generated OpenAPI)

### Web App (Frontend)
- **Pattern**: MVVM (Model-View-ViewModel)
- **Rendering**: Blazor Server + WebAssembly (Auto mode)
- **UI Framework**: Bootstrap 5
- **State Management**: ViewModels with INotifyPropertyChanged
- **API Communication**: Typed HttpClient services

## Configuration

### API Configuration

**Port Configuration** (`BlazorModularMonolith.Api/Properties/launchSettings.json`):
```json
{
  "applicationUrl": "https://localhost:7188;http://localhost:5137"
}
```

**Storage** (`BlazorModularMonolith.Api/appsettings.json`):
```json
{
  "Storage": {
    "DataDirectory": "Data"
  }
}
```

### Web App Configuration

**Port Configuration** (`BlazorModularMonolith.Web/.../Properties/launchSettings.json`):
```json
{
  "applicationUrl": "https://localhost:7031;http://localhost:5077"
}
```

**API Connection** (`BlazorModularMonolith.Web/.../appsettings.json`):
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7188"
  }
}
```

## Common Issues & Solutions

### Issue: Port Already in Use
**Solution**: Change ports in `launchSettings.json` files

### Issue: API Connection Failed in Web App
**Solution**: 
1. Verify API is running at https://localhost:7188
2. Check `appsettings.json` has correct API URL
3. Check firewall/antivirus isn't blocking connections

### Issue: CORS Errors
**Solution**: API has proper CORS configuration, but if issues arise, check the Web app's origin is allowed

### Issue: Certificate Errors
**Solution**: Trust the development certificate:
```bash
dotnet dev-certs https --trust
```

## Features

### API Features
- ✅ 24+ versioned endpoints (v1)
- ✅ People, Businesses, Addresses management
- ✅ Request/Response logging
- ✅ Global exception handling
- ✅ OpenAPI/Scalar documentation
- ✅ Thread-safe file operations
- ✅ Microservices-ready architecture

### Web App Features
- ✅ CRUD operations for People & Businesses
- ✅ Interactive forms with validation
- ✅ Loading states and error handling
- ✅ Responsive Bootstrap 5 design
- ✅ Real-time UI updates
- ✅ Proper separation with MVVM

## Documentation

- **API README**: `BlazorModularMonolith.Api/README.md`
- **Web README**: `BlazorModularMonolith.Web/README.md`
- **Architecture Guide**: `ARCHITECTURE.md`
- **API Versioning**: `API-VERSIONING.md`
- **API Testing Guide**: `API-TESTING.md`
- **Swagger Setup**: `SWAGGER-SETUP.md`

## Data Storage

Data is stored in JSON files in the `Data/` directory:
- `addresses.json` - Address records
- `people.json` - People records
- `businesses.json` - Business records

**Note**: The `Data/` folder is git-ignored. It will be created automatically on first run.

## Stopping the Applications

### If using run-separate.ps1:
- Close each terminal window

### If using run-all.ps1:
- Press `Ctrl+C` in the terminal

### If using VS Code:
- Click the Stop button in the Debug toolbar
- Or press `Shift+F5`

### If using Visual Studio:
- Press `Shift+F5`
- Or click Stop Debugging

## Next Steps

1. **Explore the API**: Visit https://localhost:7188/scalar/v1
2. **Use the Web App**: Navigate to https://localhost:7031
3. **Create a Person**: Go to People page and click "Add New Person"
4. **Create a Business**: Go to Businesses page and click "Add New Business"
5. **Add Addresses**: Create addresses and link them to people/businesses
6. **Check the Code**: Explore the clean architecture and MVVM implementation

## Contributing

This is a demonstration project showcasing:
- Modular monolith architecture
- Clean architecture principles
- MVVM pattern in Blazor
- Microservices-ready design
- Full-stack .NET 10 development

Feel free to use it as a template for your own projects!

## License

This project is open source and available under the MIT License.
