Set-StrictMode -version Latest
$ErrorActionPreference = "Stop"

Write-Host "Building SikuliSharp..." -ForegroundColor Green

# ==================================== Functions

Function GetMSBuildExe {
	Return "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
}

# ==================================== Variables

$NuGet = "$PSScriptRoot\.nuget\NuGet.exe"
$BuildPath = "$PSScriptRoot\SikuliSharp\bin\Release"
$CSProjPath = "$PSScriptRoot\SikuliSharp\SikuliSharp.csproj"

# ==================================== Build

If(Test-Path -Path $BuildPath) {
	Remove-Item -Confirm:$false "$BuildPath\*.*" -Recurse
}

&(GetMSBuildExe) SikuliSharp.sln `
	/t:Clean`;Rebuild `
	/p:Configuration=Release `
	/p:AllowedReferenceRelatedFileExtensions=- `
	/p:DebugSymbols=false `
	/p:DebugType=None `
	/clp:ErrorsOnly `
	/v:m

&($NuGet) pack $CSProjPath -Prop Configuration=Release
