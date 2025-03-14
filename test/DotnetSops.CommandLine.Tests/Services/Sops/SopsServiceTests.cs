using System.Text.Json;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Fixtures;
using NSubstitute;

namespace DotnetSops.CommandLine.Tests.Services.Sops;

[Collection(CollectionNames.Sops)]
public class SopsServiceTests : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();

    private readonly ISopsPathService _sopsPathService;

    public SopsServiceTests(SopsFixture? sopsFixture)
    {
        ArgumentNullException.ThrowIfNull(sopsFixture);

        _sopsPathService = sopsFixture.SopsPathService;
    }

    protected virtual void Dispose(bool disposing)
    {
        Environment.SetEnvironmentVariable("SOPS_AGE_KEY", null);
        _uniqueCurrentDirectoryFixture.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task EncryptAsync_Valid_Encrypts()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var sopsService = new SopsService(logger, _sopsPathService);
        var fileName = new FileInfo("secrets.json");
        var jsonContent = new { add = "Add", foo = "Rem" };
        var content = JsonSerializer.Serialize(jsonContent);
        await File.WriteAllTextAsync(
            fileName.FullName,
            content,
            TestContext.Current.CancellationToken
        );

        // Sops config
        await File.WriteAllTextAsync(
            ".sops.yaml",
            """
            creation_rules:
              - path_regex: .*.json
                age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """,
            TestContext.Current.CancellationToken
        );

        var encrypedFile = new FileInfo("encrypted.json");

        // Act
        await sopsService.EncryptAsync(
            fileName,
            encrypedFile,
            TestContext.Current.CancellationToken
        );

        // Assert
        var result = await File.ReadAllTextAsync(
            encrypedFile.FullName,
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);

        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, object>>(result);
        Assert.NotNull(jsonResult);

        Assert.StartsWith(
            "ENC",
            (jsonResult["add"] as JsonElement?)?.GetString(),
            StringComparison.Ordinal
        );
        Assert.StartsWith(
            "ENC",
            (jsonResult["foo"] as JsonElement?)?.GetString(),
            StringComparison.Ordinal
        );
        Assert.NotNull(jsonResult["sops"]);
    }

    [Fact]
    public async Task DecryptAsync_Valid_Decrypts()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var sopsService = new SopsService(logger, _sopsPathService);
        var fileName = new FileInfo("secrets.json");
        var jsonContent = new { add = "Add", foo = "Rem" };
        var content = JsonSerializer.Serialize(jsonContent);
        await File.WriteAllTextAsync(
            fileName.FullName,
            content,
            TestContext.Current.CancellationToken
        );

        // Provide age secret key for unit test purpose
        Environment.SetEnvironmentVariable(
            "SOPS_AGE_KEY",
            "AGE-SECRET-KEY-10HA9FMZENQKN8DXGZPRWZ7YK5R83AYK4FQVZ8Y5LPAV3430HXW7QZAFV9Z"
        );

        // Sops config
        await File.WriteAllTextAsync(
            ".sops.yaml",
            """
            creation_rules:
               - path_regex: .*.json
                 age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """,
            TestContext.Current.CancellationToken
        );

        var encrypedFile = new FileInfo("encrypted.json");
        await sopsService.EncryptAsync(
            fileName,
            encrypedFile,
            TestContext.Current.CancellationToken
        );

        var decrypedFile = new FileInfo("decrypted.json");

        // Act
        await sopsService.DecryptAsync(
            encrypedFile,
            decrypedFile,
            TestContext.Current.CancellationToken
        );

        // Assert
        var result = await File.ReadAllTextAsync(
            decrypedFile.FullName,
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);

        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
        Assert.NotNull(jsonResult);

        Assert.StartsWith("Add", jsonResult["add"], StringComparison.Ordinal);
        Assert.StartsWith("Rem", jsonResult["foo"], StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunCommandWithSecretsEnvironmentAsync_Valid_SecretsInEnvironment()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var sopsService = new SopsService(logger, _sopsPathService);
        var fileName = new FileInfo("secrets.json");
        var jsonContent = new { add = "Add", foo = "Rem" };
        var content = JsonSerializer.Serialize(jsonContent);
        await File.WriteAllTextAsync(
            fileName.FullName,
            content,
            TestContext.Current.CancellationToken
        );

        // Provide age secret key for unit test purpose
        Environment.SetEnvironmentVariable(
            "SOPS_AGE_KEY",
            "AGE-SECRET-KEY-10HA9FMZENQKN8DXGZPRWZ7YK5R83AYK4FQVZ8Y5LPAV3430HXW7QZAFV9Z"
        );

        // Sops config
        await File.WriteAllTextAsync(
            ".sops.yaml",
            """
            creation_rules:
               - path_regex: .*.json
                 age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """,
            TestContext.Current.CancellationToken
        );

        var encrypedFile = new FileInfo("encrypted.json");
        await sopsService.EncryptAsync(
            fileName,
            encrypedFile,
            TestContext.Current.CancellationToken
        );

        // dotnet is cross platform code that can write environment variable to file
        await File.WriteAllTextAsync(
            "Project.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """,
            TestContext.Current.CancellationToken
        );

        await File.WriteAllTextAsync(
            "Program.cs",
            """
            await File.AppendAllTextAsync("decrypted.txt", Environment.GetEnvironmentVariable("foo"));
            await File.AppendAllTextAsync("decrypted.txt", Environment.GetEnvironmentVariable("add"));
            """,
            TestContext.Current.CancellationToken
        );

        var command = "dotnet run";

        // Act
        await sopsService.RunCommandWithSecretsEnvironmentAsync(
            command,
            encrypedFile,
            TestContext.Current.CancellationToken
        );

        // Assert
        var result = await File.ReadAllTextAsync(
            "decrypted.txt",
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal("RemAdd", result);
    }

    [Fact]
    public async Task EncryptAsync_Valid_ResetPathEnvironmentVariable()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var sopsService = new SopsService(logger, _sopsPathService);
        var fileName = new FileInfo("secrets.json");
        var jsonContent = new { add = "Add", foo = "Rem" };
        var content = JsonSerializer.Serialize(jsonContent);
        await File.WriteAllTextAsync(
            fileName.FullName,
            content,
            TestContext.Current.CancellationToken
        );

        // Sops config
        await File.WriteAllTextAsync(
            ".sops.yaml",
            """
            creation_rules:
              - path_regex: .*.json
                age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """,
            TestContext.Current.CancellationToken
        );

        var encrypedFile = new FileInfo("encrypted.json");

        var pathEnvironmentBefore = Environment.GetEnvironmentVariable("PATH");

        // Act
        await sopsService.EncryptAsync(
            fileName,
            encrypedFile,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(pathEnvironmentBefore, Environment.GetEnvironmentVariable("PATH"));
    }
}
