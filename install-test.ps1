Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

dotnet pack --configuration Release

$FolderName = "ToolInstallTest"
if (Test-Path $FolderName) {
  Remove-Item $FolderName -Recurse
}
New-Item $FolderName -ItemType "directory"

Push-Location $FolderName

dotnet new tool-manifest
dotnet tool install --add-source ..\DotSops.CommandLine\bin\nupkg dotnet-sops

dotnet tool run dotnet-sops --version

dotnet dotnet-sops --version
dotnet sops --version

Pop-Location

if ($?) { 
  Exit $LASTEXITCODE 
}
