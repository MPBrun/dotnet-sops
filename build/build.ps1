$ErrorActionPreference = "Stop"

$configuration = "Release"
$timestamp = Get-Date -UFormat "%s"
$version = "0.0.0-alpha.$timestamp"

Push-Location $PSScriptRoot/..

try {
  Write-Host "Cleaning..."
  dotnet clean --configuration $configuration
  if ($LASTEXITCODE -ne 0) { 
    Write-Host "dotnet clean failed with exit code $LASTEXITCODE" -ForegroundColor red
    exit $LASTEXITCODE
  }

  Write-Host "Validating format..."
  dotnet format --no-restore --verify-no-changes
  if ($LASTEXITCODE -ne 0) { 
    Write-Host "dotnet format failed with exit code $LASTEXITCODE" -ForegroundColor red
    exit $LASTEXITCODE
  }

  Write-Host "Building..."
  dotnet build --configuration $configuration
  if ($LASTEXITCODE -ne 0) { 
    Write-Host "dotnet build failed with exit code $LASTEXITCODE" -ForegroundColor red
    exit $LASTEXITCODE
  }

  Write-Host "Testing..."
  dotnet test --no-build --configuration $configuration
  if ($LASTEXITCODE -ne 0) { 
    Write-Host "dotnet test failed with exit code $LASTEXITCODE" -ForegroundColor red
    exit $LASTEXITCODE
  }

  Write-Host "Packing..."
  dotnet pack -p:Version=$version --configuration $configuration
  if ($LASTEXITCODE -ne 0) { 
    Write-Host "dotnet pack failed with exit code $LASTEXITCODE" -ForegroundColor red
    exit $LASTEXITCODE
  }
} finally {
    Pop-Location
}