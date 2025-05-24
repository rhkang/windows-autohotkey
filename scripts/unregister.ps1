param (
    [string]$AutohotkeyScriptPath
)

. ${PSScriptRoot}/params.ps1

#if autohotkey_script_path is not provided, use the default
if (-not $AutohotkeyScriptPath) {
    $AutohotkeyScriptPath = $defaultAutohotkeyScriptPath
}

$ahkPath = Join-Path $AutohotkeyScriptPath $ahkName
$lnkPath = Join-Path $AutohotkeyScriptPath $lnkName

# Remove .ahk and .lnk from script directory
if (Test-Path $ahkPath) { Remove-Item $ahkPath }
if (Test-Path $lnkPath) { Remove-Item $lnkPath }

# Remove shortcut from Startup
$startup = [System.IO.Path]::Combine($env:APPDATA, "Microsoft\Windows\Start Menu\Programs\Startup")
$startupShortcutPath = Join-Path $startup $lnkName
if (Test-Path $startupShortcutPath) { Remove-Item $startupShortcutPath }