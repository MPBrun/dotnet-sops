namespace DotnetSops.CommandLine.Services.Sops;
internal class SopsConfiguration
{
    public required List<SopsCreationRule> CreationRules { get; init; }
}

/// <summary>
/// Based on https://github.com/getsops/sops/blob/v3.7.3/config/config.go#L112 and https://github.com/getsops/sops/blob/v3.7.3/config/config_test.go
/// </summary>
internal class SopsCreationRule
{
    public required string PathRegex { get; init; }

    public string? AzureKeyvault { get; set; }

    public string? Kms { get; set; }

    public string? GcpKms { get; set; }

    public string? Age { get; set; }

    public string? Pgp { get; set; }

    public string? HcVaultTransitUri { get; set; }

    public List<SopsKeyGroup>? KeyGroups { get; set; }
}

internal class SopsKeyGroup
{
    public List<string>? Age { get; set; }

    public List<string>? Pgp { get; set; }

    public List<SopsAzureKeyVaultKeyGroup>? AzureKeyvault { get; set; }

    public List<SopsKmsKeyGroup>? Kms { get; set; }

    public List<SopsGcpKmsKeyGroup>? GcpKms { get; set; }

    public List<string>? Vault { get; set; }
}

internal class SopsAzureKeyVaultKeyGroup
{
    [YamlDotNet.Serialization.YamlMember(Alias = "vaultUrl", ApplyNamingConventions = false)]
    public required string VaultUrl { get; set; }

    public required string Key { get; set; }

    public required string Version { get; set; }
}

internal class SopsKmsKeyGroup
{
    public required string Arn { get; set; }
}

internal class SopsGcpKmsKeyGroup
{
    public required string ResourceId { get; set; }
}
