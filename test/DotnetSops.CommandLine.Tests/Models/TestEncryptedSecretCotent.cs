namespace DotnetSops.CommandLine.Tests.Models;

public record TestEncryptedSecretCotent : EncryptedSecretCotent
{
    public required string TestKey { get; init; }
}
