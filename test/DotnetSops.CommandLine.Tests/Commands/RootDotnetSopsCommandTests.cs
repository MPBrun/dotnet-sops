using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Tests.Services;

namespace DotnetSops.CommandLine.Tests.Commands;
public class RootDotnetSopsCommandTests
{
    [Fact]
    public void Constructor_AddsSubCommands()
    {
        // Arrange / act
        var command = new RootDotnetSopsCommand(new MockServiceProvider());

        // Assert
        var subComamnds = command.Subcommands.ToList();

        Assert.Contains(subComamnds, command => command is DecryptCommand);
        Assert.Contains(subComamnds, command => command is EncryptCommand);
        Assert.Contains(subComamnds, command => command is DownloadSopsCommand);
    }
}
