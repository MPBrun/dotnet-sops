Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$DateTime = (Get-Date).ToUniversalTime()
$UnixTimeStamp = [System.Math]::Truncate((Get-Date -Date $DateTime -UFormat %s))
$VersionSuffix = "alpha-${UnixTimeStamp}"

dotnet pack --version-suffix $VersionSuffix --configuration Release

# Delete local cache. https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install#local-tools

$NugetFolder = "$env:USERPROFILE\.nuget\packages\dotnet-sops"
if (Test-Path $NugetFolder) {
  Remove-Item $NugetFolder -Recurse
}

$DotnetToolCacheFolder = "$env:USERPROFILE\.dotnet\toolResolverCache\1\dotnet-sops"
if (Test-Path $DotnetToolCacheFolder) {
  Remove-Item $DotnetToolCacheFolder
}

$FolderName = "ToolInstallTest"
if (Test-Path $FolderName) {
  Remove-Item $FolderName -Recurse
}
New-Item $FolderName -ItemType "directory"

Push-Location $FolderName

dotnet new tool-manifest
dotnet tool install --add-source ..\src\DotnetSops.CommandLine\bin\nupkg dotnet-sops --prerelease

dotnet tool run dotnet-sops -- --version

dotnet dotnet-sops --version
dotnet sops --version

Pop-Location

if ($?) { 
  Exit $LASTEXITCODE 
}
