# Windows 11 Utils
[ScreenRecording.webm](https://github.com/user-attachments/assets/2f956443-dde4-4648-ab08-b96c03d1531b)

## Windows Managing
You can modify windows size factor and move delta at `register.ps1`.

### Cascade
Ctrl + Win + c

### Resize

#### Preserve Windows Ratio
- Ctrl + Win + '+' : Upsize
- Ctrl + Win + '-' : Downsize

#### Add Width or Height
- Ctrl + Win + Alt + Arrow

### Move
Ctrl + Win + Arrow

## Install
1. Install [AutoHotKey](https://www.autohotkey.com/)
2. Install [PowerShell](https://github.com/PowerShell/PowerShell/releases)
3. Follow below

```pwsh
> cd <ProjectRoot>
> .\scripts\build.ps1   # this creates executables that autohoykey would launch
> .\scripts\register.ps1    # create ahk file and add lnk to StartUp directory
```
As source build uses c# compiler `csc.exe`, you may require to install Visual Studio or dotnet sdk. You can configure the compiler path at `build.ps1`. In default, it fetches Visual Studio's compiler.

## Uninstall
```pwsh
> cd <ProjectRoot>
> .\scripts\unregister.ps1  # remove ahk and lnk
```
