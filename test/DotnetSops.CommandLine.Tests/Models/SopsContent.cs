using System.Text.Json.Serialization;

namespace DotnetSops.CommandLine.Tests.Models;

public record SopsContent
{
    [JsonPropertyName("age")]
    public IList<AgeEncryption>? Age { get; init; }

    [JsonPropertyName("key_groups")]
    public IList<KeyGroup>? KeyGroups { get; init; }

    [JsonPropertyName("shamir_threshold")]
    public int? ShamirThreshold { get; init; }

    [JsonPropertyName("lastmodified")]
    public required DateTimeOffset Lastmodified { get; init; }

    [JsonPropertyName("mac")]
    public required string Mac { get; init; }

    [JsonPropertyName("version")]
    public required string Version { get; init; }
}

public record KeyGroup
{
    [JsonPropertyName("age")]
    public IList<AgeEncryption>? Age { get; init; }
}

public record AgeEncryption
{
    [JsonPropertyName("recipient")]
    public required string Recipient { get; init; }

    [JsonPropertyName("enc")]
    public required string Encryption { get; init; }
}
