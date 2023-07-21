using DotSops.CommandLine.Commands;
using DotSops.CommandLine.Tests.Services;

namespace DotSops.CommandLine.Tests.Commands;
public class RootDotSopsCommandTests
{
    [Fact]
    public void Constructor_AddsSubCommands()
    {
        // Arrange / act
        var command = new RootDotSopsCommand(new MockServiceProvider());

        // Assert
        var subComamnds = command.Subcommands.ToList();

        Assert.Contains(subComamnds, command => command is DecryptCommand);
        Assert.Contains(subComamnds, command => command is EncryptCommand);
        Assert.Contains(subComamnds, command => command is DownloadSopsCommand);
    }
}
