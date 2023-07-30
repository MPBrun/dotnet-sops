using System.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Services;
using Moq;
using Spectre.Console;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;
public class DownloadSopsCommandTests
{
    [Fact]
    public async Task DownloadSopsCommand_NoOptions_Success()
    {
        // Arrange
        var mockSopsDownloadService = new Mock<ISopsDownloadService>();
        using var ansiConsoleError = new TestConsole();
        using var ansiConsoleOut = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            SopsDownloadService = new Lazy<ISopsDownloadService>(mockSopsDownloadService.Object),
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsoleError),
            AnsiConsoleOut = new Lazy<IAnsiConsole>(ansiConsoleOut),
        };

        var command = new DownloadSopsCommand(serviceProvider);

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);
    }
}
