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

    public EncryptCommandTests(SopsFixture? sopsFixture)
    {
        ArgumentNullException.ThrowIfNull(sopsFixture);

        _serviceProvider = new ServiceCollection()
            .AddSingleton<ISopsService, SopsService>()
            .AddSingleton(sopsFixture.SopsPathService)
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

        var encryptedSecretContent = JsonSerializer.Deserialize<TestEncryptedSecretCotent>(await File.ReadAllTextAsync(outputPath))!;
        Assert.StartsWith("ENC[", encryptedSecretContent.TestKey, StringComparison.InvariantCulture);
        Assert.Single(encryptedSecretContent.Sops.Age!);
        Assert.Equal($"""
            User secret with ID '{id}' successfully encrypted to '{new FileInfo(outputPath).FullName}'.

            """, _logger.Out.Output, ignoreLineEndingDifferences: true);
        Assert.Equal("", _logger.Error.Output, ignoreLineEndingDifferences: true);
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
                age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8,age1y8lprkvcf2m0s2pnh866gjj4dtrazqz84kna7y3ndej0pku6ms6s84yf04
            """);

        var outputPath = "secrets.json";

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {outputPath}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.True(File.Exists(outputPath));

        var encryptedSecretContent = JsonSerializer.Deserialize<TestEncryptedSecretCotent>(await File.ReadAllTextAsync(outputPath))!;
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

        var encryptedSecretContent = JsonSerializer.Deserialize<TestEncryptedSecretCotent>(await File.ReadAllTextAsync(outputPath))!;
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
            Executing SOPS failed.

            """, _logger.Error.Output, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public async Task EncryptCommand_Defaults_FindUserSecret()
    {
        // Arrange
        var command = new EncryptCommand(_serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        await File.WriteAllTextAsync("Project.csproj", $"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net7.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <UserSecretsId>{id}</UserSecretsId>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """);

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
        var exitCode = await config.InvokeAsync($"");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.True(File.Exists(outputPath));

        var encryptedSecretContent = JsonSerializer.Deserialize<TestEncryptedSecretCotent>(await File.ReadAllTextAsync(outputPath))!;
        Assert.StartsWith("ENC[", encryptedSecretContent.TestKey, StringComparison.InvariantCulture);
        Assert.Single(encryptedSecretContent.Sops.Age!);
    }

    [Fact]
    public async Task EncryptCommand_InvalidUserSecretsFile_Fails()
    {
        // Arrange
        var command = new EncryptCommand(_serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        var outputPath = "secrets.json";

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {outputPath}");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Equal($"""
            User secrets file '{_userSecretsService.GetSecretsPathFromSecretsId(id)}' does not exist.

            You have no secrets created. You can add secrets by running this command:
              dotnet user-secrets set [name] [value]

            """, _logger.Error.Output, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public async Task EncryptCommand_DefaultsNoSecretFound_Fails()
    {
        // Arrange
        var command = new EncryptCommand(_serviceProvider);

        // user secret
        await File.WriteAllTextAsync("Project.csproj", $"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net7.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """);

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Equal($"""
            Could not find the global property 'UserSecretsId' in MSBuild project '{Path.Join(Directory.GetCurrentDirectory(), "Project.csproj")}'.

            Ensure this property is set in the project or use the '--id' command-line option.

            The 'UserSecretsId' property can be created by running this command:
              dotnet user-secrets init

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
              Encrypt existing .NET User Secrets

            Usage:
              dotnet sops encrypt [options]

            Options:
              -p, --project   Path to the project. Defaults to searching the current directory.
              --id            The user secret ID to use.
              --file          Encrypted secrets file. [default: secrets.json]
              -?, -h, --help  Show help and usage information
              --verbose       Enable verbose logging output


            """, output.ToString().RemoveHelpWrapNewLines(), ignoreLineEndingDifferences: true);
    }
}
