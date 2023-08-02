using System.CommandLine;
using System.Text.RegularExpressions;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Tests.Extensions;
using DotnetSops.CommandLine.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;
public partial class RootDotnetSopsCommandTests
{
    private readonly LoggerMock _logger = new(new TestConsole(), new TestConsole());
    private readonly IServiceProvider _serviceProvider;

    [GeneratedRegex("\\d+(?:\\.\\d+)+")]
    private static partial Regex VersionRegeex();

    public RootDotnetSopsCommandTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<ILogger>(_logger)
            .BuildServiceProvider();
    }

    [Fact]
    public void Constructor_AddsSubCommands()
    {
        // Arrange / act
        var command = new RootDotnetSopsCommand(_serviceProvider);

        // Assert
        var subComamnds = command.Subcommands.ToList();

        Assert.Contains(subComamnds, command => command is InitializeCommand);
        Assert.Contains(subComamnds, command => command is DecryptCommand);
        Assert.Contains(subComamnds, command => command is EncryptCommand);
        Assert.Contains(subComamnds, command => command is DownloadSopsCommand);
    }

    [Fact]
    public async Task VersionFlag_PrintVersion()
    {
        // Arrange
        var command = new RootDotnetSopsCommand(_serviceProvider);
        var output = new StringWriter();
        var config = new CliConfiguration(command)
        {
            Output = output
        };

        // Act
        var exitCode = await config.InvokeAsync("--version");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Matches(VersionRegeex(), output.ToString());
    }

    [Fact]
    public async Task NoVerboseFlag_NotSetAsDefault()
    {
        // Arrange
        var command = new RootDotnetSopsCommand(_serviceProvider);
        var output = new StringWriter();
        var config = new CliConfiguration(command)
        {
            Output = output
        };

        // Act
        await config.InvokeAsync("init");

        // Assert
        Assert.False(_logger.Verbose);
    }

    [Fact]
    public async Task VerboseFlag_SetVerboseLogging()
    {
        // Arrange
        var command = new RootDotnetSopsCommand(_serviceProvider);
        var output = new StringWriter();
        var config = new CliConfiguration(command)
        {
            Output = output
        };

        // Act
        await config.InvokeAsync("init --verbose");

        // Assert
        Assert.True(_logger.Verbose);
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
        var exitCode = await config.InvokeAsync(option);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Equal("""
            Description:
              Store and share encrypted secrets, created using user-secrets tool.
              Secrets are encrypted and decrypted using SOPS.
              Encrypted secrets can shared with other team members than can decrypt it, if they have access.
              
              Encryption types is configured using .sops.yaml file. Run "dotnet sops init" for help wizard to create .sops.yaml.
              
              Warning: When secrets are decrypted they are stored in plain, unencrypted text, that can be loaded by user-secrets tool.
              Recomendation: Only store development secrets that cannot access production like environment.
            
            Usage:
              dotnet sops [command] [options]
            
            Options:
              -?, -h, --help  Show help and usage information
              --version       Show version information
              --verbose       Enable verbose logging output
            
            Commands:
              init           Create .sops.yaml configuration file.
              encrypt        Encrypt existing dotnet user secrets
              decrypt        Decrypt secrets into dotnet user secrets
              download-sops  Download SOPS from https://github.com/getsops/sops


            """, output.ToString().RemoveHelpWrapNewLines(), ignoreLineEndingDifferences: true);
    }
}
