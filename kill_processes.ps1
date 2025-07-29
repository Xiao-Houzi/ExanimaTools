#!/usr/bin/env pwsh

Write-Host "Forcefully terminating all ExanimaTools related processes..."

# Kill by process name
$processNames = @("ExanimaTools", "dotnet")
foreach ($name in $processNames) {
    try {
        Get-Process -Name $name -ErrorAction SilentlyContinue | ForEach-Object {
            Write-Host "Killing $($_.Name) (PID: $($_.Id))"
            $_.Kill()
        }
    } catch {
        Write-Host "Failed to kill $name processes: $_"
    }
}

# Kill by command line containing ExanimaTools
try {
    Get-WmiObject Win32_Process | Where-Object { 
        $_.CommandLine -like "*ExanimaTools*" -or 
        $_.CommandLine -like "*dotnet*run*" -or
        $_.CommandLine -like "*ExanimaTools.dll*"
    } | ForEach-Object {
        Write-Host "Killing process by command line: $($_.Name) (PID: $($_.ProcessId)) - $($_.CommandLine)"
        try {
            $_.Terminate()
        } catch {
            Write-Host "Failed to terminate PID $($_.ProcessId): $_"
        }
    }
} catch {
    Write-Host "Failed to query WMI processes: $_"
}

# Use taskkill as final resort
Write-Host "Using taskkill as backup..."
cmd /c "taskkill /F /IM dotnet.exe 2>nul"
cmd /c "taskkill /F /IM ExanimaTools.exe 2>nul"

Write-Host "Process termination complete. You should now be able to run the application."
