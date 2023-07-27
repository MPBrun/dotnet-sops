using System.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Tests.Services;
using Spectre.Console;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;
public class InitializeCommandTests
{
    [Fact]
    public async Task InitializeCommand_CreateFile()
    {
        using var ansiConsole = new TestConsole();
        ansiConsole.Profile.Capabilities.Interactive = true;

        var serviceProvider = new MockServiceProvider()
        {
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new InitializeCommand(serviceProvider);

        var config = new CliConfiguration(command);

        // Owerwrite existing file
        ansiConsole.Input.PushTextWithEnter("y");

        // Select age encryption
        ansiConsole.Input.PushKey(ConsoleKey.DownArrow);
        ansiConsole.Input.PushKey(ConsoleKey.DownArrow);
        ansiConsole.Input.PushKey(ConsoleKey.DownArrow);
        ansiConsole.Input.PushKey(ConsoleKey.DownArrow);
        ansiConsole.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        ansiConsole.Input.PushTextWithEnter("age123");

        // Act
        var exitCode = await config.InvokeAsync("");

        Assert.Contains("? Are you sure to overwrite existing .sops.yaml? [y/n]: y", ansiConsole.Output, StringComparison.InvariantCulture);
        Assert.Contains("? Which encryption whould you like to use? age", ansiConsole.Output, StringComparison.InvariantCulture);
        Assert.Contains("? What is public key of age? age123", ansiConsole.Output, StringComparison.InvariantCulture);
        Assert.Contains(
            """
            Generated .sops.yaml with the following content:
            creation_rules:
            - path_regex: secrets.json
              age: age123
            """, ansiConsole.Output.ReplaceLineEndings(), StringComparison.InvariantCulture);
        Assert.Equal(0, exitCode);
    }
}
