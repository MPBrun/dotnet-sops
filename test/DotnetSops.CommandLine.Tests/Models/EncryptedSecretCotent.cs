using System.Text.Json.Serialization;

namespace DotnetSops.CommandLine.Tests.Models;
public record EncryptedSecretCotent
{
    [JsonPropertyName("sops")]
    public required SopsContent Sops { get; init; }
}
