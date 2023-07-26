using System.CommandLine;
using DotSops.CommandLine.Commands;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Tests.Services;
using Moq;
using Spectre.Console;
using Spectre.Console.Testing;

namespace DotSops.CommandLine.Tests.Commands;
public class DownloadSopsCommandTests
{
    [Fact]
    public async Task DownloadSopsCommand_NoOptions_Success()
    {
        // Arrange
        var mockSopsDownloadService = new Mock<ISopsDownloadService>();
        using var ansiConsole = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            SopsDownloadService = new Lazy<ISopsDownloadService>(mockSopsDownloadService.Object),
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new DownloadSopsCommand(serviceProvider);

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);
    }
}
