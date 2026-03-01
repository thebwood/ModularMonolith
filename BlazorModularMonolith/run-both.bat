@echo off
echo ========================================
echo Address Management System
echo ========================================
echo.
echo Starting API and Blazor Web App...
echo.

REM Start API in new window
echo Starting API at https://localhost:7188
start "API Server" cmd /k "cd BlazorModularMonolith.Api && dotnet run"

REM Wait 3 seconds
timeout /t 3 /nobreak > nul

REM Start Web App in new window
echo Starting Blazor Web at https://localhost:7031
start "Blazor Web App" cmd /k "cd BlazorModularMonolith.Web\BlazorModularMonolith.Web && dotnet run"

echo.
echo ========================================
echo Both applications launched!
echo ========================================
echo.
echo URLs:
echo   API:        https://localhost:7188
echo   API Docs:   https://localhost:7188/scalar/v1
echo   Web App:    https://localhost:7031
echo.
echo Close the command windows to stop the applications
echo.
pause
