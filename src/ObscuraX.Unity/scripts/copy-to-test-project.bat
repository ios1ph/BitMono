@echo off
echo ObscuraX Unity - Setup Test Project
echo Current directory: %CD%

set "TEST_PROJECT=..\..\..\test\ObscuraX.Unity.TestProject\Assets\ObscuraX.Unity"

REM Clean and create directories
if exist "%TEST_PROJECT%" rmdir /s /q "%TEST_PROJECT%"
mkdir "%TEST_PROJECT%"
mkdir "%TEST_PROJECT%\Editor"

REM Copy files from main source to test project
copy "..\Editor\*.cs" "%TEST_PROJECT%\Editor\" /Y
copy "..\Editor\*.asmdef" "%TEST_PROJECT%\Editor\" /Y
REM Copy .meta files if they exist (to preserve GUIDs)
if exist "..\Editor\*.cs.meta" copy "..\Editor\*.cs.meta" "%TEST_PROJECT%\Editor\" /Y
if exist "..\Editor\*.asmdef.meta" copy "..\Editor\*.asmdef.meta" "%TEST_PROJECT%\Editor\" /Y
copy "..\package.json" "%TEST_PROJECT%\" /Y
copy "..\README.md" "%TEST_PROJECT%\" /Y

REM Copy ObscuraXConfig.asset (and .meta if exists) to preserve GUID/script binding
if exist "..\ObscuraXConfig.asset" copy "..\ObscuraXConfig.asset" "%TEST_PROJECT%\" /Y
if exist "..\ObscuraXConfig.asset.meta" copy "..\ObscuraXConfig.asset.meta" "%TEST_PROJECT%\" /Y

REM Static .meta is committed; no generation needed

REM Build ObscuraX.CLI if needed
if not exist "..\..\..\src\ObscuraX.CLI\bin\Release\net462\ObscuraX.CLI.exe" (
    echo Building ObscuraX.CLI...
    dotnet build "..\..\..\src\ObscuraX.CLI\ObscuraX.CLI.csproj" --configuration Release
)

REM Copy ObscuraX.CLI into Assets (will be disabled via PluginImporter)
set "CLI_BASE=..\..\..\src\ObscuraX.CLI\bin\Release\net462"
set "CLI_SOURCE=%CLI_BASE%\win-x64"
if not exist "%CLI_SOURCE%\ObscuraX.CLI.exe" (
    set "CLI_SOURCE=%CLI_BASE%"
)
set "CLI_DEST=%TEST_PROJECT%\ObscuraX.CLI"

if not exist "%CLI_DEST%" mkdir "%CLI_DEST%"

if exist "%CLI_SOURCE%\ObscuraX.CLI.exe" (
    echo Copying ObscuraX.CLI from %CLI_SOURCE% to %CLI_DEST%
    xcopy "%CLI_SOURCE%\*" "%CLI_DEST%\" /E /I /Y
    if %ERRORLEVEL% EQU 0 (
        echo ObscuraX.CLI copied successfully into Assets (import disabled by editor script)
        echo Generating .meta files to disable plugin import for CLI DLLs...
        for /R "%CLI_DEST%" %%F in (*.dll) do (
            >"%%~fF.meta" echo fileFormatVersion: 2
            >>"%%~fF.meta" echo guid: %%~nF000000000000000000000000000000
            >>"%%~fF.meta" echo PluginImporter:
            >>"%%~fF.meta" echo ^  serializedVersion: 2
            >>"%%~fF.meta" echo ^  isPreloaded: 0
            >>"%%~fF.meta" echo ^  isOverridable: 0
            >>"%%~fF.meta" echo ^  platformData:
            >>"%%~fF.meta" echo ^  - first:
            >>"%%~fF.meta" echo ^      Any:
            >>"%%~fF.meta" echo ^    second:
            >>"%%~fF.meta" echo ^      enabled: 0
            >>"%%~fF.meta" echo ^  userData:
            >>"%%~fF.meta" echo ^  assetBundleName:
            >>"%%~fF.meta" echo ^  assetBundleVariant:
        )
        echo .meta generation complete.
    ) else (
        echo ERROR: Failed to copy ObscuraX.CLI
    )
) else (
    echo ERROR: ObscuraX.CLI not found at %CLI_SOURCE%
)

echo ✅ Done! Open Unity and refresh (Ctrl+R)
pause
