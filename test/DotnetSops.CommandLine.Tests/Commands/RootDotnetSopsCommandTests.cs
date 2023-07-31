using DotnetSops.CommandLine.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSops.CommandLine.Tests.Commands;
public class RootDotnetSopsCommandTests
{
    private readonly IServiceProvider _serviceProvider;

    public RootDotnetSopsCommandTests()
    {
        _serviceProvider = new ServiceCollection()
            .BuildServiceProvider();
    }

    [Fact]
    public void Constructor_AddsSubCommands()
    {
        // Arrange / act
        var command = new RootDotnetSopsCommand(_serviceProvider);

        // Assert
        var subComamnds = command.Subcommands.ToList();

        Assert.Contains(subComamnds, command => command is DecryptCommand);
        Assert.Contains(subComamnds, command => command is EncryptCommand);
        Assert.Contains(subComamnds, command => command is DownloadSopsCommand);
    }
}
