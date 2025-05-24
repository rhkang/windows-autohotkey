# csc.exe can be found in the Roslyn folder of the MSBuild installation directory or in the .NET SDK installation directory.
$compilerPath = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe"

. ${PSScriptRoot}/params.ps1

$srcDir = "${projectRoot}/src"

# execute the compiler
& $compilerPath /nologo /out:util.exe $srcDir/*cs