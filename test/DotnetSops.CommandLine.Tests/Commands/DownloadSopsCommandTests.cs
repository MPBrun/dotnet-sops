using System.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Extensions;
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class DownloadSopsCommandTests : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();
    private readonly LoggerMock _logger;
    private readonly ISopsDownloadService _mockSopsDownloadService;
    private readonly IServiceProvider _serviceProvider;

    public DownloadSopsCommandTests()
    {
        _logger = new LoggerMock(new TestConsole(), new TestConsole());
        _mockSopsDownloadService = Substitute.For<ISopsDownloadService>();

        _serviceProvider = new ServiceCollection()
            .AddSingleton(_mockSopsDownloadService)
            .AddSingleton<ILogger>(_logger)
            .BuildServiceProvider();
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
    public async Task DownloadSopsCommand_NoOptions_Success()
    {
        // Arrange
        var command = new DownloadSopsCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.StartsWith(
            "Downloading SOPS from https://github.com/getsops/sops",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Equal("SOPS has been successfully downloaded.\n", _logger.Out.Output);
    }

    [Fact]
    public async Task DownloadSopsCommand_DownloadThrows_LogError()
    {
        // Arrange
        var command = new DownloadSopsCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        _mockSopsDownloadService
            .DownloadAsync()
            .ThrowsAsyncForAnyArgs(
                new SopsExecutionException("Expcetion message") { ExitCode = 1 }
            );

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(1, exitCode);
        Assert.StartsWith(
            "Downloading SOPS from https://github.com/getsops/sops",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            "Expcetion message\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
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
        var exitCode = await config.InvokeAsync($"download-sops {option}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Equal(
            """
            Description:
              Download SOPS from https://github.com/getsops/sops

            Usage:
              dotnet sops download-sops [options]

            Options:
              -?, -h, --help  Show help and usage information
              --verbose       Enable verbose logging output


            """,
            output.ToString().RemoveHelpWrapNewLines(),
            ignoreLineEndingDifferences: true
        );
    }
}
