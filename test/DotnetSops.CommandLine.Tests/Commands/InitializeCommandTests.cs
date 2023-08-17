using System.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Tests.Extensions;
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class InitializeCommandTests : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();
    private readonly LoggerMock _logger = new(new TestConsole(), new TestConsole());
    private readonly IServiceProvider _serviceProvider;

    public InitializeCommandTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<ILogger>(_logger)
            .BuildServiceProvider();
    }

    protected virtual void Dispose(bool disposing)
    {
        _uniqueCurrentDirectoryFixture.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task InitializeCommand_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        await File.WriteAllTextAsync(".sops.yaml", "");

        // Owerwrite existing file
        _logger.Error.Input.PushTextWithEnter("y");

        // No key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Select age encryption
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter("age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8");

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        Assert.Contains("? Are you sure you want to overwrite the existing .sops.yaml? [y/n] (n): y", _logger.Error.Output, StringComparison.InvariantCulture);
        Assert.Contains("? Use key groups? [y/n] (n): n", _logger.Error.Output, StringComparison.InvariantCulture);
        Assert.Contains("? Which key type would you like to use?", _logger.Error.Output, StringComparison.InvariantCulture);
        Assert.Contains("? What is the public key of age? age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8", _logger.Error.Output, StringComparison.InvariantCulture);
        Assert.Contains("? Add more keys? [y/n] (n): n", _logger.Error.Output, StringComparison.InvariantCulture);
        Assert.Contains(
            """
            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """, _logger.Error.Output.ReplaceLineEndings(), StringComparison.InvariantCulture);
        Assert.Equal(0, exitCode);
    }

    [Theory]
    [InlineData("-?")]
    [InlineData("-h")]
    [InlineData("--help")]
    public async Task HelpFlag_PrintHelp(string option)
    {
        // Arrange
        var command = new RootDotnetSopsCommand(_serviceProvider);
        var output = new StringWriter();
        var config = new CliConfiguration(command)
        {
            Output = new ReplaceUsageHelpTextWriter(output, "testhost")
        };

        // Act
        var exitCode = await config.InvokeAsync($"init {option}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Equal("""
            Description:
              Create a .sops.yaml configuration file.

            Usage:
              dotnet sops init [options]

            Options:
              -?, -h, --help  Show help and usage information
              --verbose       Enable verbose logging output


            """, output.ToString().RemoveHelpWrapNewLines(), ignoreLineEndingDifferences: true);
    }
}
