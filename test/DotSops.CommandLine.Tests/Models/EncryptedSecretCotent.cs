using System.Text.Json.Serialization;

namespace DotSops.CommandLine.Tests.Models;
public record EncryptedSecretCotent
{
    [JsonPropertyName("sops")]
    public required SopsContent Sops { get; init; }
}
