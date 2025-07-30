#!/usr/bin/env pwsh
# Quick test script to launch the app and test Arsenal -> Pool -> Loadout flow

Write-Host "Testing ExanimaTools Arsenal Equipment Flow" -ForegroundColor Green

# Navigate to the application directory
Set-Location "E:\Dev\GameSupport\Exanima\ExanimaTools\bin\Debug\net9.0"

# Check if the application exists
if (Test-Path ".\ExanimaTools.exe") {
    Write-Host "Starting ExanimaTools..." -ForegroundColor Yellow
    Start-Process ".\ExanimaTools.exe" -PassThru
    Write-Host "Application started. Test the following flow:" -ForegroundColor Cyan
    Write-Host "1. Go to Company tab" -ForegroundColor White
    Write-Host "2. Select a company member" -ForegroundColor White
    Write-Host "3. Go to Equipment tab" -ForegroundColor White
    Write-Host "4. Look for 'Add to Personal Pool' buttons on Arsenal items" -ForegroundColor White
    Write-Host "5. Click to move item to Personal Pool" -ForegroundColor White
    Write-Host "6. Look for 'Assign to Rank' buttons on Personal Pool items" -ForegroundColor White
    Write-Host "7. Select a rank and assign equipment" -ForegroundColor White
} else {
    Write-Host "Application not found. Building first..." -ForegroundColor Red
    Set-Location "E:\Dev\GameSupport\Exanima"
    dotnet build
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Build successful. Starting application..." -ForegroundColor Green
        Set-Location "ExanimaTools\bin\Debug\net9.0"
        Start-Process ".\ExanimaTools.exe" -PassThru
    } else {
        Write-Host "Build failed." -ForegroundColor Red
    }
}
