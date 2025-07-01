using System.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class RunCommandTests_SopsMissing : IDisposable
{
    private const string PathEnvironmentVariableName = "PATH";
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();
    private readonly LoggerMock _logger = new(new TestConsole(), new TestConsole());
    private readonly IServiceProvider _serviceProvider;
    private readonly string? _originalPathEnviromentVariableValue;

    public RunCommandTests_SopsMissing()
    {
        _originalPathEnviromentVariableValue = Environment.GetEnvironmentVariable(
            PathEnvironmentVariableName
        );

        var sopsPathService = Substitute.For<ISopsPathService>();
        sopsPathService.GetDotnetSopsUserDirectory().Returns("");

        _serviceProvider = new ServiceCollection()
            .AddSingleton<ISopsService, SopsService>()
            .AddSingleton(sopsPathService)
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
    public async Task RunCommand_MissingSops_OutputSopsError()
    {
        // Arrange
        var command = new RunCommand(_serviceProvider);

        Environment.SetEnvironmentVariable(PathEnvironmentVariableName, null);

        var inputPath = "secrets.json";

        var config = new CommandLineConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync(
            $"arg1 arg2 arg3 --configuration Release --file {inputPath}"
        );

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
