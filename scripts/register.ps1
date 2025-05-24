param (
    [string]$AutohotkeyScriptPath
)

. ${PSScriptRoot}/params.ps1

#if autohotkey_script_path is not provided, use the default
if (-not $AutohotkeyScriptPath) {
    $AutohotkeyScriptPath = $defaultAutohotkeyScriptPath
}

$exeDir = $projectRoot
$text = @"
#Requires AutoHotkey v2.0

cascadeWindowSizeRatio := 0.66
resizeRatioDelta := 0.05
moveStep := 50
resizeAddDelta := 50

^#c::  ; Cascade windows
{
    Run("$exeDir\util.exe cascade " cascadeWindowSizeRatio "", , "Hide")
}

^#=::  ; Upsize active window
{
    Run("$exeDir\util.exe up " resizeRatioDelta "", , "Hide")
}

^#-::  ; Downsize active window
{
    Run("$exeDir\util.exe down " resizeRatioDelta "", , "Hide")
}

^#Right::  ; move right
{
    Run("$exeDir\util.exe move " moveStep " 0", , "Hide")
}

^#Left::  ; move left
{
    Run("$exeDir\util.exe move -" moveStep " 0", , "Hide")
}

^#Up::  ; move up
{
    Run("$exeDir\util.exe move 0 -" moveStep "", , "Hide")
}

^#Down::  ; move down
{
    Run("$exeDir\util.exe move 0 " moveStep "", , "Hide")
}

^#!Up::  ; height decrease
{
    Run("$exeDir\util.exe add 0 -" resizeAddDelta "", , "Hide")
}

^#!Down::  ; height increase
{
    Run("$exeDir\util.exe add 0 " resizeAddDelta "", , "Hide")
}

^#!Left::  ; width decrease
{
    Run("$exeDir\util.exe add -" resizeAddDelta " 0", , "Hide")
}

^#!Right::  ; width increase
{
    Run("$exeDir\util.exe add " resizeAddDelta " 0", , "Hide")
}
"@

if (-not (Test-Path $AutohotkeyScriptPath)) {
    New-Item -Path $AutohotkeyScriptPath -ItemType Directory | Out-Null
}

$outFile = Join-Path $AutohotkeyScriptPath $ahkName
Set-Content -Path $outFile -Value $text -Encoding UTF8

# create lnk
$shortcutPath = Join-Path $AutohotkeyScriptPath $lnkName
$targetPath = Join-Path $AutohotkeyScriptPath $ahkName
$workingDirectory = $AutohotkeyScriptPath
$shortcut = (New-Object -ComObject WScript.Shell).CreateShortcut($shortcutPath)
$shortcut.TargetPath = $targetPath
$shortcut.WorkingDirectory = $workingDirectory
$shortcut.IconLocation = $targetPath
$shortcut.Save()

#move lnk to startup
$startup = [System.IO.Path]::Combine($env:APPDATA, "Microsoft\Windows\Start Menu\Programs\Startup")
$startupShortcutPath = Join-Path $startup $lnkName
if (Test-Path $startupShortcutPath) {
    Remove-Item $startupShortcutPath
}
Move-Item -Path $shortcutPath -Destination $startupShortcutPath