Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-Dotnet {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true)]
        [System.String]
        $Command
    )

    $DotnetArgs = @()
    $DotnetArgs = $DotnetArgs + ($Command -split "\s+")
    dotnet $DotnetArgs

    # Should throw if the last command failed.
    if ($LASTEXITCODE -ne 0) {
        throw "Invoking 'dotnet $DotnetArgs' failed"
    }
}

$DateTime = (Get-Date).ToUniversalTime()
$UnixTimeStamp = [System.Math]::Truncate((Get-Date -Date $DateTime -UFormat %s))
$VersionSuffix = "alpha-${UnixTimeStamp}"

Write-Host ""
Write-Host "Build dotnet-sops tool"
Invoke-Dotnet -Command "pack --version-suffix $VersionSuffix --configuration Release"

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
New-Item $FolderName -ItemType "directory" | Out-Null

Push-Location $FolderName

try {
  Write-Host ""
  Write-Host "Testing as local tool"
  Invoke-Dotnet -Command "new tool-manifest"
  Invoke-Dotnet -Command "tool install --add-source ..\src\DotnetSops.CommandLine\bin\nupkg dotnet-sops --prerelease"
  Invoke-Dotnet -Command "tool run dotnet-sops -- --version"
  Invoke-Dotnet -Command "dotnet-sops --version"
  Invoke-Dotnet -Command "sops --version"
  Invoke-Dotnet -Command "sops download-sops"
  Invoke-Dotnet -Command "tool uninstall dotnet-sops"

  Write-Host ""
  Write-Host "Testing as global tool"
  Invoke-Dotnet -Command "tool install --add-source ..\src\DotnetSops.CommandLine\bin\nupkg dotnet-sops --prerelease --global"
  Invoke-Dotnet -Command "sops --version"
  Invoke-Dotnet -Command "sops download-sops"
  Invoke-Dotnet -Command "tool uninstall dotnet-sops --global"
} finally {
  Pop-Location
}