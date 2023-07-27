using System.CommandLine;
using DotSops.CommandLine.Commands;
using DotSops.CommandLine.Tests.Services;
using Spectre.Console;
using Spectre.Console.Testing;

namespace DotSops.CommandLine.Tests.Commands;
public class InitializeCommandTests
{
    [Fact]
    public async Task InitializeCommand_CreateFile()
    {
        using var ansiConsole = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new InitializeCommand(serviceProvider);

        var config = new CliConfiguration(command);

        AnsiConsole.Record();

        // Act
        var exitCodeTask = config.InvokeAsync("");

        ansiConsole.Input.PushTextWithEnter("y");

        var exitCode = await exitCodeTask;

        Assert.Equal("asd", ansiConsole.Output);
        Assert.Equal(0, exitCode);
    }
}
