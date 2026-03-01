# Simple script to run both projects in separate terminal windows

Write-Host "Starting Address Management System in separate windows..." -ForegroundColor Green
Write-Host ""

# Start API in new window
Write-Host "Launching API (https://localhost:7188)..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\BlazorModularMonolith.Api'; Write-Host 'API Server' -ForegroundColor Green; dotnet run"

# Wait a moment
Start-Sleep -Seconds 2

# Start Web App in new window
Write-Host "Launching Blazor Web (https://localhost:7031)..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\BlazorModularMonolith.Web\BlazorModularMonolith.Web'; Write-Host 'Blazor Web App' -ForegroundColor Green; dotnet run"

Write-Host ""
Write-Host "Both applications launched in separate windows!" -ForegroundColor Green
Write-Host ""
Write-Host "URLs:" -ForegroundColor Yellow
Write-Host "  API:        https://localhost:7188" -ForegroundColor White
Write-Host "  API Docs:   https://localhost:7188/scalar/v1" -ForegroundColor White
Write-Host "  Web App:    https://localhost:7031" -ForegroundColor White
Write-Host ""
Write-Host "Close the terminal windows to stop the applications" -ForegroundColor Gray
