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
    [Fact]
    public async Task DownloadSopsCommand_NoOptions_Success()
    {
        // Arrange
        var mockSopsDownloadService = new Mock<ISopsDownloadService>();
        var logger = new LoggerMock(new TestConsole(), new TestConsole());

        var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSopsDownloadService.Object)
                .AddSingleton<ILogger>(logger)
                .BuildServiceProvider();

        var command = new DownloadSopsCommand(serviceProvider);

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);
    }
}
