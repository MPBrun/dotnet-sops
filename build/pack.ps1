$ErrorActionPreference = "Stop"

$configuration = "Release"
$timestamp = Get-Date -UFormat "%s"
$version = "0.0.0-alpha.$timestamp"

Push-Location $PSScriptRoot/..

try {
  dotnet pack -p:Version=$version --configuration $configuration
  if ($LASTEXITCODE -ne 0) { 
    Write-Host "dotnet pack failed with exit code $LASTEXITCODE" -ForegroundColor red
    exit $LASTEXITCODE
  }
} finally {
  Pop-Location
}