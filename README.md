# Windows 11 Utils

## Windows Managing
You can modify move delta or resize ratio delta at `register.ps1`.

### Cascade
Ctrl + Win + Alt + c

### Resize
- Ctrl + Win + Alt + '+' : Upsize
- Ctrl + Win + Alt + '-' : Downsize

### Move
Ctrl + Win + Alt + Arrow

## Install
1. Install [AutoHotKey](https://www.autohotkey.com/)
2. Install [PowerShell](https://github.com/PowerShell/PowerShell/releases)
3. Follow below

```pwsh
> cd <ProjectRoot>
> .\scripts\build.ps1   # this creates executables that autohoykey would launch
> .\scripts\register.ps1    # create ahk file and add lnk to StartUp directory
```

## Uninstall
```pwsh
> cd <ProjectRoot>
> .\scripts\unregister.ps1  # remove ahk and lnk
```