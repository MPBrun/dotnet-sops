using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Services;
using DotnetSops.CommandLine.Tests.Services.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class DecryptCommandTests_SopsMissing : IDisposable
{
    private const string PathEnvironmentVariableName = "PATH";
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();
    private readonly LoggerMock _logger = new(new TestConsole(), new TestConsole());
    private readonly IServiceProvider _serviceProvider;
    private readonly string? _originalPathEnviromentVariableValue;

    public DecryptCommandTests_SopsMissing()
    {
        _originalPathEnviromentVariableValue = Environment.GetEnvironmentVariable(
            PathEnvironmentVariableName
        );

        var sopsPathService = Substitute.For<ISopsPathService>();
        sopsPathService.GetDotnetSopsUserDirectory().Returns("");

        _serviceProvider = new ServiceCollection()
            .AddSingleton<ISopsService, SopsService>()
            .AddSingleton(sopsPathService)
            .AddSingleton<IUserSecretsService>(sp => new UserSecretsServiceStub(
                _uniqueCurrentDirectoryFixture.TestDirectory.FullName
            ))
            .AddSingleton<IFileBomService, FileBomService>()
            .AddSingleton<IPlatformInformationService, PlatformInformationService>()
            .AddSingleton<IProjectInfoService, ProjectInfoService>()
            .AddSingleton<ILogger>(_logger)
            .BuildServiceProvider();
    }

    protected virtual void Dispose(bool disposing)
    {
        _uniqueCurrentDirectoryFixture.Dispose();
        Environment.SetEnvironmentVariable(
            PathEnvironmentVariableName,
            _originalPathEnviromentVariableValue
        );
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DecryptCommand_MissingSops_OutputSopsError()
    {
        // Arrange
        var command = new DecryptCommand(_serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        Environment.SetEnvironmentVariable(PathEnvironmentVariableName, null);

        await File.WriteAllTextAsync("secrets.json", "", TestContext.Current.CancellationToken);

        var inputPath = "secrets.json";

        // Act
        var exitCode = await command.Parse($"--id {id} --file {inputPath}").InvokeAsync();

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Equal(
            """
            SOPS executable could not be found on the PATH.

            You can download it by executing the following command:
              dotnet sops download-sops

            """,
            _logger.Error.Output,
            ignoreLineEndingDifferences: true
        );
    }
}
