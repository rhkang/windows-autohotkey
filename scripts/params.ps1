$projectRoot = (Resolve-Path -Path "${PSScriptRoot}/../").Path
$defaultAutohotkeyScriptPath = [System.IO.Path]::GetFullPath((Resolve-Path -Path "~/Documents/AutoHotkey").Path)
$scriptName = "windows11 util"
$ahkName = "${scriptName}.ahk"
$lnkName = "${scriptName} - Shortcut.lnk"