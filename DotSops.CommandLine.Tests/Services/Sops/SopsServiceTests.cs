using System.Text.Json;
using DotSops.CommandLine.Services.Sops;

namespace DotSops.CommandLine.Tests.Services.Sops;
public class SopsServiceTests
{
    [Fact]
    public void DefaultConsturctor_WorkingDirectory_IsCurrentDirectory()
    {
        // Arrange / Act
        var service = new SopsService();

        // Assert
        Assert.Equal(Directory.GetCurrentDirectory(), service.WorkingDirectory);
    }

    [Fact]
    public void InternalConsturctor_WorkingDirectory_Set()
    {
        // Arrange
        var workingDir = Guid.NewGuid().ToString();

        // Act
        var service = new SopsService(workingDir);

        // Assert
        Assert.Equal(workingDir, service.WorkingDirectory);
    }

    [Fact]
    public async Task EncryptAsync_Valid_Encrypts()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var sopsService = new SopsService(dir.FullName);
        var fileName = new FileInfo(Path.Combine(dir.FullName, "secrets.json"));
        var jsonContent = new
        {
            add = "Add",
            foo = "Rem"
        };
        var content = JsonSerializer.Serialize(jsonContent);
        await File.WriteAllTextAsync(fileName.FullName, content);

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: .*.json
                  age: 'age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8'
            """);

        var encrypedFile = new FileInfo(Path.Combine(dir.FullName, "encrypted.json"));

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
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var sopsService = new SopsService(dir.FullName);
        var fileName = new FileInfo(Path.Combine(dir.FullName, "secrets.json"));
        var jsonContent = new
        {
            add = "Add",
            foo = "Rem"
        };
        var content = JsonSerializer.Serialize(jsonContent);
        await File.WriteAllTextAsync(fileName.FullName, content);

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: .*.json
                  age: 'age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8'
            """);

        var encrypedFile = new FileInfo(Path.Combine(dir.FullName, "encrypted.json"));
        await sopsService.EncryptAsync(fileName, encrypedFile);

        var decrypedFile = new FileInfo(Path.Combine(dir.FullName, "decrypted.json"));

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
