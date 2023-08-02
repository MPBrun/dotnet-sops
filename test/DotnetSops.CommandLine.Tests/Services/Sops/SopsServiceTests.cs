using System.Text.Json;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Fixtures;
using Moq;

namespace DotnetSops.CommandLine.Tests.Services.Sops;

[Collection(CollectionNames.Sops)]
public class SopsServiceTests : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();

    public SopsServiceTests(SopsFixture sopsFixture)
    {
        sopsFixture?.CopySopsToDirectory(_uniqueCurrentDirectoryFixture.TestDirectory.FullName);
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
        var logger = new Mock<ILogger>();
        var sopsService = new SopsService(logger.Object);
        var fileName = new FileInfo("secrets.json");
        var jsonContent = new
        {
            add = "Add",
            foo = "Rem"
        };
        var content = JsonSerializer.Serialize(jsonContent);
        await File.WriteAllTextAsync(fileName.FullName, content);

        // Sops config
        await File.WriteAllTextAsync(".sops.yaml", """
            creation_rules:
                - path_regex: .*.json
                  age: 'age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8'
            """);

        var encrypedFile = new FileInfo("encrypted.json");

        // Act
        await sopsService.EncryptAsync(fileName, encrypedFile);

        // Assert
        var result = await File.ReadAllTextAsync(encrypedFile.FullName);
        Assert.NotNull(result);

        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, object>>(result);
        Assert.NotNull(jsonResult);

        Assert.StartsWith("ENC", (jsonResult["add"] as JsonElement?)?.GetString(), StringComparison.Ordinal);
        Assert.StartsWith("ENC", (jsonResult["foo"] as JsonElement?)?.GetString(), StringComparison.Ordinal);
        Assert.NotNull(jsonResult["sops"]);
    }

    [Fact]
    public async Task DecryptAsync_Valid_Decrypts()
    {
        // Arrange
        var logger = new Mock<ILogger>();
        var sopsService = new SopsService(logger.Object);
        var fileName = new FileInfo("secrets.json");
        var jsonContent = new
        {
            add = "Add",
            foo = "Rem"
        };
        var content = JsonSerializer.Serialize(jsonContent);
        await File.WriteAllTextAsync(fileName.FullName, content);

        // Provide age secret key for unit test purpose
        Environment.SetEnvironmentVariable("SOPS_AGE_KEY", "AGE-SECRET-KEY-10HA9FMZENQKN8DXGZPRWZ7YK5R83AYK4FQVZ8Y5LPAV3430HXW7QZAFV9Z");

        // Sops config
        await File.WriteAllTextAsync(".sops.yaml", """
            creation_rules:
                - path_regex: .*.json
                  age: 'age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8'
            """);

        var encrypedFile = new FileInfo("encrypted.json");
        await sopsService.EncryptAsync(fileName, encrypedFile);

        var decrypedFile = new FileInfo("decrypted.json");

        // Act
        await sopsService.DecryptAsync(encrypedFile, decrypedFile);

        // Assert
        var result = await File.ReadAllTextAsync(decrypedFile.FullName);
        Assert.NotNull(result);

        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
        Assert.NotNull(jsonResult);

        Assert.StartsWith("Add", jsonResult["add"], StringComparison.Ordinal);
        Assert.StartsWith("Rem", jsonResult["foo"], StringComparison.Ordinal);
    }
}
