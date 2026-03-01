# Run Both API and Blazor Web App

Write-Host "Starting Address Management System..." -ForegroundColor Green
Write-Host ""

# Start API in background
Write-Host "Starting API at https://localhost:7188..." -ForegroundColor Cyan
$apiJob = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    Set-Location "BlazorModularMonolith.Api"
    dotnet run
}

# Wait a moment for API to start
Start-Sleep -Seconds 3

# Start Web App in background
Write-Host "Starting Blazor Web at https://localhost:7031..." -ForegroundColor Cyan
$webJob = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    Set-Location "BlazorModularMonolith.Web\BlazorModularMonolith.Web"
    dotnet run
}

Write-Host ""
Write-Host "Both applications are starting..." -ForegroundColor Green
Write-Host "API:        https://localhost:7188" -ForegroundColor Yellow
Write-Host "API Docs:   https://localhost:7188/scalar/v1" -ForegroundColor Yellow
Write-Host "Web App:    https://localhost:7031" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press Ctrl+C to stop both applications" -ForegroundColor White
Write-Host ""

# Monitor jobs and display output
try {
    while ($true) {
        # Get output from both jobs
        $apiOutput = Receive-Job -Job $apiJob
        $webOutput = Receive-Job -Job $webJob
        
        if ($apiOutput) {
            Write-Host "[API] $apiOutput" -ForegroundColor Cyan
        }
        if ($webOutput) {
            Write-Host "[WEB] $webOutput" -ForegroundColor Magenta
        }
        
        # Check if jobs are still running
        if ($apiJob.State -eq 'Failed' -or $webJob.State -eq 'Failed') {
            Write-Host "One or more applications failed to start" -ForegroundColor Red
            break
        }
        
        Start-Sleep -Milliseconds 500
    }
}
finally {
    Write-Host ""
    Write-Host "Stopping applications..." -ForegroundColor Yellow
    Stop-Job -Job $apiJob -ErrorAction SilentlyContinue
    Stop-Job -Job $webJob -ErrorAction SilentlyContinue
    Remove-Job -Job $apiJob -Force -ErrorAction SilentlyContinue
    Remove-Job -Job $webJob -Force -ErrorAction SilentlyContinue
    Write-Host "Applications stopped" -ForegroundColor Green
}
