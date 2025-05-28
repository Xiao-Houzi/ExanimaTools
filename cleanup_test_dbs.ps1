# PowerShell script to clean up all test DB files in the workspace
$patterns = @(
    "TestArsenal_*.db",
    "TestEquipmentVM.db",
    "TestArsenal.db",
    "TestEquipment.db",
    "TestTeamMember.db"
)
$folders = @(
    ".",
    ".\ETModels.Tests",
    ".\ExanimaTools.Persistence",
    ".\ExanimaToolsApp"
)
foreach ($folder in $folders) {
    foreach ($pattern in $patterns) {
        Get-ChildItem -Path $folder -Filter $pattern -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
    }
}
Write-Host "Test DB cleanup complete."
