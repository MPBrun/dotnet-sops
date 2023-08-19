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
$Version = "0.0.0-alpha.${UnixTimeStamp}"
$Configuration = "Release"

Write-Host ""
Write-Host "Build dotnet-sops tool"
Invoke-Dotnet -Command "pack -p:Version=$Version --configuration $Configuration"
