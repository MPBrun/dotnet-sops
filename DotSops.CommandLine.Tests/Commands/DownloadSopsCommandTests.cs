using System.CommandLine;
using System.CommandLine.Invocation;
using DotSops.CommandLine.Commands;
using DotSops.CommandLine.Services.Sops;
using Moq;
using Spectre.Console;

namespace DotSops.CommandLine.Tests.Commands;
public class DownloadSopsCommandTests
{
    [Fact]
    public async Task DownloadSopsCommand_NoOptions_Success()
    {
        // Arrange
        var mockSopsDownloadService = new Mock<ISopsDownloadService>();
        var command = new DownloadSopsCommand();

        var parseResult = command.Parse($"");
        using var invocationContext = new InvocationContext(parseResult);
        invocationContext.BindingContext.AddService(typeof(IAnsiConsole), sp => AnsiConsole.Console);
        invocationContext.BindingContext.AddService(typeof(ISopsDownloadService), sp => mockSopsDownloadService.Object);

        // Act
        var exitCode = await command.Handler!.InvokeAsync(invocationContext);

        // Assert
        Assert.Equal(0, exitCode);
    }
}
