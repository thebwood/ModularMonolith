# Run Both Projects - Setup Summary

## ✅ What Was Configured

Your workspace is now set up to run both the API and Blazor Web App together!

### 📁 Files Created

1. **.vscode/launch.json** - VS Code debug configurations
   - API: Launch - Debug API only
   - Web: Launch - Debug Web App only
   - **API + Web: Launch Both** - Debug both together (⭐ Use this!)

2. **.vscode/tasks.json** - Build tasks
   - build-api
   - build-web
   - build-all
   - watch-api (hot reload)
   - watch-web (hot reload)

3. **run-all.ps1** - PowerShell script (single terminal)
   - Runs both in background with job management
   - Colored output: [API] cyan, [WEB] magenta
   - Press Ctrl+C to stop both

4. **run-separate.ps1** - PowerShell script (separate windows)
   - Opens two terminal windows
   - One for API, one for Web
   - Easy to see output separately

5. **run-both.bat** - Windows batch file
   - Simple double-click to run both
   - Opens two command prompts
   - No PowerShell required

6. **RUN-BOTH-PROJECTS.md** - Complete documentation
   - All methods to run both projects
   - Troubleshooting guide
   - Configuration details

### 🎯 Solution File Updated

**BlazorModularMonolith.slnx** now includes:
- ✅ BlazorModularMonolith.Api
- ✅ BlazorModularMonolith.Web
- ✅ BlazorModularMonolith.Web.Client (auto-included)

## 🚀 How to Run Both Projects

### Method 1: PowerShell Script (Separate Windows) ⭐ Easiest
```powershell
.\run-separate.ps1
```
- Opens two terminal windows
- API in one window, Web in another
- See each application's output clearly

### Method 2: PowerShell Script (Single Terminal)
```powershell
.\run-all.ps1
```
- Both run in background
- Colored output in one window
- Press Ctrl+C to stop both

### Method 3: Batch File (Windows)
```cmd
run-both.bat
```
- Double-click to run
- Opens two command prompts
- Simple and straightforward

### Method 4: Visual Studio Code
1. Press `F5`
2. Select **"API + Web: Launch Both"**
3. Both projects start with debugging

### Method 5: Visual Studio 2022
1. Open `BlazorModularMonolith.slnx`
2. Right-click solution → Set Startup Projects
3. Select Multiple startup projects
4. Set both to Start
5. Press F5

### Method 6: Manual (Two Terminals)
**Terminal 1:**
```bash
cd BlazorModularMonolith.Api
dotnet run
```

**Terminal 2:**
```bash
cd BlazorModularMonolith.Web/BlazorModularMonolith.Web
dotnet run
```

## 🌐 Application URLs

Once running, access:

| App | URL | Description |
|-----|-----|-------------|
| **API** | https://localhost:7188 | REST API |
| **API Docs** | https://localhost:7188/scalar/v1 | Interactive docs (auto-opens) |
| **Web App** | https://localhost:7031 | Blazor UI |

## 📊 Port Configuration

### API Ports
- HTTPS: `7188`
- HTTP: `5137`

### Web App Ports
- HTTPS: `7031`
- HTTP: `5077`

**No port conflicts!** Both can run simultaneously.

## 🔧 VS Code Debug Configuration

The compound launch configuration:

```json
{
  "name": "API + Web: Launch Both",
  "configurations": ["API: Launch", "Web: Launch"],
  "stopAll": true
}
```

**Features:**
- ✅ Starts both projects
- ✅ Debugging enabled for both
- ✅ Stop All - Stops both with one click
- ✅ Separate debug sessions

## 🎯 Recommended Workflow

### For Daily Development:

1. **Start both:**
   ```powershell
   .\run-separate.ps1
   ```

2. **API will auto-open** Scalar documentation

3. **Navigate to Web App** at https://localhost:7031

4. **Make changes** to either project

5. **Stop both** by closing the terminal windows

### For Debugging:

1. **Open VS Code**

2. **Press F5** or click Run and Debug

3. **Select "API + Web: Launch Both"**

4. **Set breakpoints** in either project

5. **Debug both** simultaneously

### For Hot Reload:

**Terminal 1:**
```bash
cd BlazorModularMonolith.Api
dotnet watch run
```

**Terminal 2:**
```bash
cd BlazorModularMonolith.Web/BlazorModularMonolith.Web
dotnet watch run
```

Changes auto-reload without restart!

## ✅ Quick Test

After starting both:

1. **Check API is running:**
   - Visit https://localhost:7188/scalar/v1
   - Should see Scalar API documentation

2. **Check Web App is running:**
   - Visit https://localhost:7031
   - Should see the home page

3. **Test integration:**
   - Go to People page
   - Click "Add New Person"
   - Fill out form
   - Click Save
   - Should see new person in table

4. **Verify data:**
   - Check `Data/people.json` file
   - Should contain the new person record

## 🛑 Stopping Applications

### PowerShell Scripts:
- **run-all.ps1**: Press `Ctrl+C`
- **run-separate.ps1**: Close terminal windows
- **run-both.bat**: Close command prompts

### VS Code:
- Click Stop button in Debug toolbar
- Or press `Shift+F5`

### Manual:
- Press `Ctrl+C` in each terminal

## 📝 Configuration Files

### API Configuration
- **Port**: `BlazorModularMonolith.Api/Properties/launchSettings.json`
- **Settings**: `BlazorModularMonolith.Api/appsettings.json`
- **Data storage**: Configured in appsettings (default: `Data/`)

### Web App Configuration
- **Port**: `BlazorModularMonolith.Web/.../Properties/launchSettings.json`
- **API URL**: `BlazorModularMonolith.Web/.../appsettings.json`
- **Current API URL**: `https://localhost:7188`

## 🎨 PowerShell Script Features

### run-all.ps1:
```
[API] Starting at https://localhost:7188...
[WEB] Starting at https://localhost:7031...

API:        https://localhost:7188
API Docs:   https://localhost:7188/scalar/v1
Web App:    https://localhost:7031

Press Ctrl+C to stop both applications
```

### run-separate.ps1:
```
Launching API (https://localhost:7188)...
Launching Blazor Web (https://localhost:7031)...

Both applications launched in separate windows!
```

## 📚 Documentation

- **Main README**: `README.md` (API overview)
- **Web README**: `BlazorModularMonolith.Web/README.md`
- **Run Guide**: `RUN-BOTH-PROJECTS.md` ⭐ Comprehensive guide
- **Architecture**: `ARCHITECTURE.md`
- **API Testing**: `API-TESTING.md`

## 🎉 Summary

You now have **5 different ways** to run both projects:

1. ⭐ **run-separate.ps1** - Recommended for daily use
2. **run-all.ps1** - Compact single terminal
3. **run-both.bat** - Double-click simplicity
4. **VS Code F5** - Debugging enabled
5. **Manual** - Full control

**Everything is configured and ready to go!**

Just run one of the scripts or press F5 in VS Code, and both your API and Blazor Web App will start together! 🚀
