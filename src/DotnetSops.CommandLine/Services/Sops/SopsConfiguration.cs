namespace DotnetSops.CommandLine.Services.Sops;

internal class SopsConfiguration
{
    public List<SopsCreationRule>? CreationRules { get; set; }
}

/// <summary>
/// Based on https://github.com/getsops/sops/blob/v3.7.3/config/config.go#L112 and https://github.com/getsops/sops/blob/v3.7.3/config/config_test.go
/// </summary>
internal class SopsCreationRule
{
    public string? PathRegex { get; set; }

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
    public string? VaultUrl { get; set; }

    public string? Key { get; set; }

    public string? Version { get; set; }
}

internal class SopsKmsKeyGroup
{
    public string? Arn { get; set; }
}

internal class SopsGcpKmsKeyGroup
{
    public string? ResourceId { get; set; }
}
