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
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Models;
using DotnetSops.CommandLine.Tests.Services;
using DotnetSops.CommandLine.Tests.Services.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class EncryptCommandTests_SopsMissing : IDisposable
{
    private const string PathEnvironmentVariableName = "PATH";
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();
    private readonly LoggerMock _logger = new(new TestConsole(), new TestConsole());
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserSecretsService _userSecretsService;
    private readonly string? _originalPathEnviromentVariableValue;

    public EncryptCommandTests_SopsMissing()
    {
        _originalPathEnviromentVariableValue = Environment.GetEnvironmentVariable(PathEnvironmentVariableName);

        var sopsPathService = Substitute.For<ISopsPathService>();
        sopsPathService.GetDotnetSopsUserDirectory().Returns("");

        _serviceProvider = new ServiceCollection()
            .AddSingleton<ISopsService, SopsService>()
            .AddSingleton(sopsPathService)
            .AddSingleton<IUserSecretsService>(sp => new UserSecretsServiceStub(_uniqueCurrentDirectoryFixture.TestDirectory.FullName))
            .AddSingleton<IFileBomService, FileBomService>()
            .AddSingleton<IPlatformInformationService, PlatformInformationService>()
            .AddSingleton<IProjectInfoService, ProjectInfoService>()
            .AddSingleton<ILogger>(_logger)
            .BuildServiceProvider();

        _userSecretsService = _serviceProvider.GetRequiredService<IUserSecretsService>();
    }

    protected virtual void Dispose(bool disposing)
    {
        _uniqueCurrentDirectoryFixture.Dispose();
        Environment.SetEnvironmentVariable(PathEnvironmentVariableName, _originalPathEnviromentVariableValue);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task EncryptCommand_MissingSops_OutputSopsError()
    {
        // Arrange
        var command = new EncryptCommand(_serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        Environment.SetEnvironmentVariable(PathEnvironmentVariableName, null);

        // user secret
        var filePath = _userSecretsService.GetSecretsPathFromSecretsId(id);
        var secrets = new TestSecretCotent
        {
            TestKey = "test value"
        };
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(secrets), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        var outputPath = "secrets.json";

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {outputPath}");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Equal("""
            SOPS executable could not be found on the PATH.

            You can download it by executing the following command:
              dotnet sops download-sops

            """, _logger.Error.Output, ignoreLineEndingDifferences: true);
    }
}
