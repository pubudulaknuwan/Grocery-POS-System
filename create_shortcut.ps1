# Create Desktop Shortcut for VillageSmartPOS
Write-Host "Creating Desktop Shortcut for VillageSmartPOS..." -ForegroundColor Green

# Get the current directory
$CurrentDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$AppPath = Join-Path $CurrentDir "bin\Release\net8.0-windows\VillageSmartPOS.exe"
$DesktopPath = [Environment]::GetFolderPath("Desktop")
$ShortcutPath = Join-Path $DesktopPath "VillageSmartPOS.lnk"

# Check if the application exists
if (-not (Test-Path $AppPath)) {
    Write-Host "Error: Application not found at $AppPath" -ForegroundColor Red
    Write-Host "Please build the application first using: dotnet build -c Release" -ForegroundColor Yellow
    Read-Host "Press Enter to continue"
    exit 1
}

# Create the shortcut
$WshShell = New-Object -ComObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut($ShortcutPath)
$Shortcut.TargetPath = $AppPath
$Shortcut.WorkingDirectory = Split-Path $AppPath
$Shortcut.Description = "VillageSmartPOS - Point of Sale System"
$Shortcut.IconLocation = "$AppPath,0"
$Shortcut.Save()

# Check if shortcut was created successfully
if (Test-Path $ShortcutPath) {
    Write-Host "Desktop shortcut created successfully!" -ForegroundColor Green
    Write-Host "Shortcut location: $ShortcutPath" -ForegroundColor Cyan
    Write-Host "You can now double-click the shortcut on your desktop to run VillageSmartPOS" -ForegroundColor Yellow
} else {
    Write-Host "Failed to create desktop shortcut." -ForegroundColor Red
}

Read-Host "Press Enter to continue" 