using System.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;
public class DownloadSopsCommandTests
{
    private readonly IServiceProvider _serviceProvider;

    public DownloadSopsCommandTests()
    {
        var logger = new LoggerMock(new TestConsole(), new TestConsole());
        var mockSopsDownloadService = new Mock<ISopsDownloadService>();

        _serviceProvider = new ServiceCollection()
                .AddSingleton(mockSopsDownloadService.Object)
                .AddSingleton<ILogger>(logger)
                .BuildServiceProvider();
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
        Assert.Equal("""
            Description:
              Download SOPS from https://github.com/getsops/sops

            Usage:
              dotnet sops download-sops [options]

            Options:
              -?, -h, --help  Show help and usage information
              --verbose       Enable verbose logging output



            """, output.ToString(), ignoreLineEndingDifferences: true);
    }
}
