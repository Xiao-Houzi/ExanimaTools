#!/usr/bin/env pwsh

# Test script to check if the application can start
Write-Host "Testing ExanimaTools startup..."

$exe = "e:\Dev\GameSupport\Exanima\ExanimaTools\bin\Debug\net9.0\ExanimaTools.exe"

if (Test-Path $exe) {
    Write-Host "Executable exists: $exe"
    
    # Try to run the executable and capture any output
    Write-Host "Attempting to start application..."
    
    # Start the process and immediately kill it to see if it starts
    $process = Start-Process -FilePath $exe -PassThru -NoNewWindow
    Start-Sleep -Seconds 2
    
    if (!$process.HasExited) {
        Write-Host "Process started successfully (PID: $($process.Id))"
        $process.Kill()
        Write-Host "Process terminated"
    } else {
        Write-Host "Process exited immediately with code: $($process.ExitCode)"
    }
} else {
    Write-Host "Executable not found: $exe"
}

# Check for new log files
$logDir = "e:\Dev\GameSupport\Exanima\ExanimaTools\bin\Debug\net9.0\logs"
if (Test-Path $logDir) {
    Write-Host "Log files:"
    Get-ChildItem $logDir -File | Sort-Object LastWriteTime -Descending | Select-Object -First 3 | ForEach-Object {
        Write-Host "  $($_.Name) - $($_.LastWriteTime)"
    }
} else {
    Write-Host "Log directory not found"
}
