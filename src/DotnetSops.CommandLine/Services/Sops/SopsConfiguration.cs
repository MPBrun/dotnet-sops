namespace DotnetSops.CommandLine.Services.Sops;
internal class SopsConfiguration
{
    public required List<SopsCreationRule> CreationRules { get; init; }
}

/// <summary>
/// Based on https://github.com/getsops/sops/blob/v3.7.3/config/config.go#L112
/// </summary>
internal class SopsCreationRule
{
    public required string PathRegex { get; init; }

    public string? AzureKeyvault { get; set; }

    public string? AwsProfile { get; set; }

    public string? Kms { get; set; }

    public string? GcpKms { get; set; }

    public string? Age { get; set; }

    public string? Pgp { get; set; }

    public string? HcVaultTransitUri { get; set; }
}
