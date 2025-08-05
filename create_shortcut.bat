@echo off
echo Creating Desktop Shortcut for VillageSmartPOS...

:: Get the current directory
set "CURRENT_DIR=%~dp0"
set "APP_PATH=%CURRENT_DIR%bin\Release\net8.0-windows\VillageSmartPOS.exe"
set "DESKTOP_PATH=%USERPROFILE%\Desktop"
set "SHORTCUT_PATH=%DESKTOP_PATH%\VillageSmartPOS.lnk"

:: Create VBS script to create shortcut
echo Set oWS = WScript.CreateObject("WScript.Shell") > CreateShortcut.vbs
echo sLinkFile = "%SHORTCUT_PATH%" >> CreateShortcut.vbs
echo Set oLink = oWS.CreateShortcut(sLinkFile) >> CreateShortcut.vbs
echo oLink.TargetPath = "%APP_PATH%" >> CreateShortcut.vbs
echo oLink.WorkingDirectory = "%CURRENT_DIR%bin\Release\net8.0-windows" >> CreateShortcut.vbs
echo oLink.Description = "VillageSmartPOS - Point of Sale System" >> CreateShortcut.vbs
echo oLink.IconLocation = "%APP_PATH%,0" >> CreateShortcut.vbs
echo oLink.Save >> CreateShortcut.vbs

:: Run the VBS script
cscript //nologo CreateShortcut.vbs

:: Clean up
del CreateShortcut.vbs

if exist "%SHORTCUT_PATH%" (
    echo Desktop shortcut created successfully!
    echo Shortcut location: %SHORTCUT_PATH%
) else (
    echo Failed to create desktop shortcut.
)

pause 