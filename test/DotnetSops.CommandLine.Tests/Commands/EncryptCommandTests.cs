using System.CommandLine;
using System.Text;
using System.Text.Json;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Models;
using DotnetSops.CommandLine.Tests.Services;
using DotnetSops.CommandLine.Tests.Services.UserSecrets;
using Spectre.Console;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.Sops)]
public class EncryptCommandTests
{
    [Fact]
    public async Task EncryptCommand_ValidOptions_CreateFile()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var projectInfoService = new ProjectInfoService();
        var sopsService = new SopsService(dir.FullName);
        var userSecretsService = new UserSecretsServiceStub(dir.FullName);
        var fileBomService = new FileBomService();
        using var ansiConsole = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            ProjectInfoService = new Lazy<IProjectInfoService>(projectInfoService),
            SopsService = new Lazy<ISopsService>(sopsService),
            UserSecretsService = new Lazy<IUserSecretsService>(userSecretsService),
            FileBomService = new Lazy<IFileBomService>(fileBomService),
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new EncryptCommand(serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """);

        var outputPath = Path.Combine(dir.FullName, "secrets.json");

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {outputPath}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.True(File.Exists(outputPath));

        var encryptedSecretContent = JsonSerializer.Deserialize<TestEncryptedSecretCotent>(File.ReadAllText(outputPath))!;
        Assert.StartsWith("ENC[", encryptedSecretContent.TestKey, StringComparison.InvariantCulture);
        Assert.Single(encryptedSecretContent.Sops.Age!);
    }

    [Fact]
    public async Task EncryptCommand_ValidOptionsMultiple_CreateFile()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var projectInfoService = new ProjectInfoService();
        var sopsService = new SopsService(dir.FullName);
        var userSecretsService = new UserSecretsServiceStub(dir.FullName);
        var fileBomService = new FileBomService();
        using var ansiConsole = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            ProjectInfoService = new Lazy<IProjectInfoService>(projectInfoService),
            SopsService = new Lazy<ISopsService>(sopsService),
            UserSecretsService = new Lazy<IUserSecretsService>(userSecretsService),
            FileBomService = new Lazy<IFileBomService>(fileBomService),
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new EncryptCommand(serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  age: >-
                    age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8,
                    age1y8lprkvcf2m0s2pnh866gjj4dtrazqz84kna7y3ndej0pku6ms6s84yf04
            """);

        var outputPath = Path.Combine(dir.FullName, "secrets.json");

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {outputPath}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.True(File.Exists(outputPath));

        var encryptedSecretContent = JsonSerializer.Deserialize<TestEncryptedSecretCotent>(File.ReadAllText(outputPath))!;
        Assert.StartsWith("ENC[", encryptedSecretContent.TestKey, StringComparison.InvariantCulture);
        Assert.Equal(2, encryptedSecretContent.Sops.Age?.Count);
    }

    [Fact]
    public async Task EncryptCommand_ValidOptionsKeyGroups_CreateFile()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var projectInfoService = new ProjectInfoService();
        var sopsService = new SopsService(dir.FullName);
        var userSecretsService = new UserSecretsServiceStub(dir.FullName);
        var fileBomService = new FileBomService();
        using var ansiConsole = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            ProjectInfoService = new Lazy<IProjectInfoService>(projectInfoService),
            SopsService = new Lazy<ISopsService>(sopsService),
            UserSecretsService = new Lazy<IUserSecretsService>(userSecretsService),
            FileBomService = new Lazy<IFileBomService>(fileBomService),
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new EncryptCommand(serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  shamir_threshold: 2
                  key_groups:
                    - age:
                        - age1y8lprkvcf2m0s2pnh866gjj4dtrazqz84kna7y3ndej0pku6ms6s84yf04
                    - age:
                        - age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
                        - age10l04egantyrujnu4ll0unhxkd6m4krardf4kqnfg4uqcczyawcts9pgtlm
                    - age:
                        - age1kg6yjfq8ce9k7u4yjnd3jsp5hkkghxmc65raafrkxlcrtlmz8u7qv57ehh
            """);

        var outputPath = Path.Combine(dir.FullName, "secrets.json");

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {outputPath}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.True(File.Exists(outputPath));

        var encryptedSecretContent = JsonSerializer.Deserialize<TestEncryptedSecretCotent>(File.ReadAllText(outputPath))!;
        Assert.StartsWith("ENC[", encryptedSecretContent.TestKey, StringComparison.InvariantCulture);
        Assert.Equal(3, encryptedSecretContent.Sops.KeyGroups?.Count);
        Assert.Single(encryptedSecretContent.Sops.KeyGroups![0].Age!);
        Assert.Equal(2, encryptedSecretContent.Sops.KeyGroups![1].Age!.Count);
        Assert.Single(encryptedSecretContent.Sops.KeyGroups![2].Age!);
        Assert.Equal(2, encryptedSecretContent.Sops.ShamirThreshold);
    }

    [Fact]
    public async Task EncryptCommand_InvalidAgeKey_OutputSopsError()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var projectInfoService = new ProjectInfoService();
        var sopsService = new SopsService(dir.FullName);
        var userSecretsService = new UserSecretsServiceStub(dir.FullName);
        var fileBomService = new FileBomService();
        using var ansiConsole = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            ProjectInfoService = new Lazy<IProjectInfoService>(projectInfoService),
            SopsService = new Lazy<ISopsService>(sopsService),
            UserSecretsService = new Lazy<IUserSecretsService>(userSecretsService),
            FileBomService = new Lazy<IFileBomService>(fileBomService),
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new EncryptCommand(serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  age: invalid
            """);

        var outputPath = Path.Combine(dir.FullName, "secrets.json");

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {outputPath}");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Equal("""
            SOPS failed with error.

            failed to parse input as Bech32-encoded age public key: malformed recipient 
            "invalid": separator '1' at invalid position: pos=-1, len=7

            """, ansiConsole.Output, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void EncryptCommand_Id_NotRequired()
    {
        // Arrange
        var command = new EncryptCommand(new MockServiceProvider());

        // Act / Assert
        var option = command.Options.First(o => o.Name == "--id");
        Assert.False(option.Required);
    }

    [Fact]
    public void EncryptCommand_File_NotRequired()
    {
        // Arrange
        var command = new EncryptCommand(new MockServiceProvider());

        // Act / Assert
        var option = command.Options.First(o => o.Name == "--file");
        Assert.False(option.Required);
    }
}
