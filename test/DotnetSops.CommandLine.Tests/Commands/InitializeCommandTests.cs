using System.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;
public class InitializeCommandTests
{
    private readonly LoggerMock _logger = new(new TestConsole(), new TestConsole());
    private readonly IServiceProvider _serviceProvider;

    public InitializeCommandTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<ILogger>(_logger)
            .BuildServiceProvider();
    }

    [Fact]
    public async Task InitializeCommand_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // Owerwrite existing file
        _logger.Error.Input.PushTextWithEnter("y");

        // Select age encryption
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter("age123");

        // Act
        var exitCode = await config.InvokeAsync("");

        Assert.Contains("? Are you sure to overwrite existing .sops.yaml? [y/n]: y", _logger.Error.Output, StringComparison.InvariantCulture);
        Assert.Contains("? Which encryption whould you like to use? age", _logger.Error.Output, StringComparison.InvariantCulture);
        Assert.Contains("? What is public key of age? age123", _logger.Error.Output, StringComparison.InvariantCulture);
        Assert.Contains(
            """
            Generated .sops.yaml with the following content:
            creation_rules:
            - path_regex: secrets.json
              age: age123
            """, _logger.Error.Output.ReplaceLineEndings(), StringComparison.InvariantCulture);
        Assert.Equal(0, exitCode);
    }
}
