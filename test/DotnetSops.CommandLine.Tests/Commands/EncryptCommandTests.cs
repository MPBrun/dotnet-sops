using System.CommandLine;
using System.Text;
using System.Text.Json;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using DotnetSops.CommandLine.Tests.Extensions;
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Models;
using DotnetSops.CommandLine.Tests.Services;
using DotnetSops.CommandLine.Tests.Services.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.Sops)]
public class EncryptCommandTests : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();
    private readonly LoggerMock _logger = new(new TestConsole(), new TestConsole());
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserSecretsService _userSecretsService;

    public EncryptCommandTests(SopsFixture sopsFixture)
    {
        sopsFixture?.CopySopsToDirectory(_uniqueCurrentDirectoryFixture.TestDirectory.FullName);

        _serviceProvider = new ServiceCollection()
            .AddSingleton<ISopsService, SopsService>()
            .AddSingleton<IUserSecretsService>(sp => new UserSecretsServiceStub(_uniqueCurrentDirectoryFixture.TestDirectory.FullName))
            .AddSingleton<IFileBomService, FileBomService>()
            .AddSingleton<ISopsDownloadService, SopsDownloadService>()
            .AddSingleton<IPlatformInformationService, PlatformInformationService>()
            .AddSingleton<IProjectInfoService, ProjectInfoService>()
            .AddSingleton<ILogger>(_logger)
            .BuildServiceProvider();

        _userSecretsService = _serviceProvider.GetRequiredService<IUserSecretsService>();
    }

    protected virtual void Dispose(bool disposing)
    {
        _uniqueCurrentDirectoryFixture.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task EncryptCommand_ValidOptions_CreateFile()
    {
        // Arrange
        var command = new EncryptCommand(_serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = _userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = ".sops.yaml";
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """);

        var outputPath = "secrets.json";

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
        var command = new EncryptCommand(_serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = _userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = ".sops.yaml";
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  age: >-
                    age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8,
                    age1y8lprkvcf2m0s2pnh866gjj4dtrazqz84kna7y3ndej0pku6ms6s84yf04
            """);

        var outputPath = "secrets.json";

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
        var command = new EncryptCommand(_serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = _userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = ".sops.yaml";
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

        var outputPath = "secrets.json";

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
        var command = new EncryptCommand(_serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = _userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = ".sops.yaml";
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  age: invalid
            """);

        var outputPath = "secrets.json";

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {outputPath}");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Equal("""
            SOPS failed with error:

            failed to parse input as Bech32-encoded age public key: malformed recipient 
            "invalid": separator '1' at invalid position: pos=-1, len=7

            """, _logger.Error.Output, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void EncryptCommand_Id_NotRequired()
    {
        // Arrange
        var command = new EncryptCommand(_serviceProvider);

        // Act / Assert
        var option = command.Options.First(o => o.Name == "--id");
        Assert.False(option.Required);
    }

    [Fact]
    public void EncryptCommand_File_NotRequired()
    {
        // Arrange
        var command = new EncryptCommand(_serviceProvider);

        // Act / Assert
        var option = command.Options.First(o => o.Name == "--file");
        Assert.False(option.Required);
    }

    [Theory]
    [InlineData("-?")]
    [InlineData("-h")]
    [InlineData("--help")]
    public async Task HelpFlag_PrintHelp(string option)
    {
        // Arrange
        var command = new RootDotnetSopsCommand(_serviceProvider);
        var output = new StringWriter();
        var config = new CliConfiguration(command)
        {
            Output = new ReplaceUsageHelpTextWriter(output, "testhost")
        };

        // Act
        var exitCode = await config.InvokeAsync($"encrypt {option}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Equal("""
            Description:
              Encrypt existing dotnet user secrets

            Usage:
              dotnet sops encrypt [options]

            Options:
              -p, --project   Path to project. Defaults to searching the current directory
              --id            The user secret ID to use.
              --file          File of the encrypted secrets [default: secrets.json]
              -?, -h, --help  Show help and usage information
              --verbose       Enable verbose logging output



            """, output.ToString().RemoveHelpWrapNewLines(), ignoreLineEndingDifferences: true);
    }
}
