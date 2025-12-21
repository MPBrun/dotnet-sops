using System.Text.Json.Serialization;

namespace DotnetSops.CommandLine.Tests.Models;

public record TestSecretCotent
{
    public required string TestKey { get; init; }
}

public record FooTestContent
{
    public required string Add { get; init; }

    public required string Foo { get; init; }
}

public record EncryptedFooTestContent : FooTestContent
{
    [JsonPropertyName("sops")]
    public required SopsContent Sops { get; init; }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(TestSecretCotent))]
[JsonSerializable(typeof(FooTestContent))]
[JsonSerializable(typeof(EncryptedFooTestContent))]
[JsonSerializable(typeof(TestEncryptedSecretCotent))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext { }
