using System.CommandLine;
using System.Reflection;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Extensions;
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.Sops)]
public class RunCommandTests : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();
    private readonly LoggerMock _logger = new(new TestConsole(), new TestConsole());
    private readonly IServiceProvider _serviceProvider;

    public RunCommandTests(SopsFixture? sopsFixture)
    {
        ArgumentNullException.ThrowIfNull(sopsFixture);

        _serviceProvider = new ServiceCollection()
            .AddSingleton<ISopsService, SopsService>()
            .AddSingleton(sopsFixture.SopsPathService)
            .AddSingleton<ILogger>(_logger)
            .BuildServiceProvider();
    }

    protected virtual void Dispose(bool disposing)
    {
        _uniqueCurrentDirectoryFixture.Dispose();
        Environment.SetEnvironmentVariable("SOPS_AGE_KEY", null);
        Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", null);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void RunCommand_File_NotRequired()
    {
        // Arrange
        var command = new RunCommand(_serviceProvider);

        // Act / Assert
        var option = command.Options.First(o => o.Name == "--file");
        Assert.False(option.Required);
    }

    [Fact]
    public async Task RunCommand_ValidOptions_ExecuteDotnetRun()
    {
        // Arrange
        var command = new RunCommand(_serviceProvider);

        // Provide age secret key for unit test purpose
        Environment.SetEnvironmentVariable(
            "SOPS_AGE_KEY",
            "AGE-SECRET-KEY-10HA9FMZENQKN8DXGZPRWZ7YK5R83AYK4FQVZ8Y5LPAV3430HXW7QZAFV9Z"
        );

        // Sops config
        await File.WriteAllTextAsync(
            ".sops.yaml",
            """
            creation_rules:
              - path_regex: secrets.json
                age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """,
            TestContext.Current.CancellationToken
        );

        await File.WriteAllTextAsync(
            "secrets.json",
            /*lang=json,strict*/
            """
            {
                "TestKey": "ENC[AES256_GCM,data:L3MRIGiBVhqFcA==,iv:+57aY2xTo6lwwVaUF2ifbvgs5uPT0xsmwPFlWRexFrg=,tag:b3v7JRAhLxZRCgblwGOZvg==,type:str]",
                "sops": {
                    "kms": null,
                    "gcp_kms": null,
                    "azure_kv": null,
                    "hc_vault": null,
                    "age": [
                        {
                            "recipient": "age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8",
                            "enc": "-----BEGIN AGE ENCRYPTED FILE-----\nYWdlLWVuY3J5cHRpb24ub3JnL3YxCi0+IFgyNTUxOSB1ZkdnUlhidzV1bjdObDE1\nbXZSYVpObExxVnE1WEVrNnN6cDE2WUpUSjNZCmJzYktWRkFhT2RwOFF0N01mMFEv\nNXF3TmVzZE1CczNHblNLNU5CVWVBVE0KLS0tIFNXam5rOVozR3RHaFlCRkJnVEVv\nQ2t6VDNHVUJJTXZTVXVMNzVKaVkyTG8K2owMkncIH4WDJ3IG0v0hO8e9/LLhbFAk\nK4jc3emW/DBRQwvtGkGMOa/WKOV29R3t4TYbaHxeLmVIF1u+43rmfQ==\n-----END AGE ENCRYPTED FILE-----\n"
                        }
                    ],
                    "lastmodified": "2023-07-21T08:21:43Z",
                    "mac": "ENC[AES256_GCM,data:r6KgqW5MSMf7NUAusF8foeIHTd2UkwnACqi5CPZbE28F9qhyWbtlC1NHwThGB0Jd0cOAigHmRLTAjodx6g+sStllqjAVnQQtFY4lyw1jbA1YkDx8ICB7WV3CytJBZJ7ZdQbH8gSqJMScDVAbVQb+UUi5+cipVjpaJiWU4UdNtVI=,iv:Rnr0pqpufMtaYoOe3O7DYsv73Fta2aTb9IHqbap9gFs=,tag:IHUNSDnca6vKeTEDjywb+w==,type:str]",
                    "pgp": null,
                    "unencrypted_suffix": "_unencrypted",
                    "version": "3.7.3"
                }
            }
            """,
            TestContext.Current.CancellationToken
        );

        await File.WriteAllTextAsync(
            "Project.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """,
            TestContext.Current.CancellationToken
        );

        await File.WriteAllTextAsync(
            "Program.cs",
            """
            // Return 0 if environment variable is equal expected
            var valid = true;
            #if DEBUG
            valid = false; //Verify is release configuration
            #endif
            valid &= Environment.GetEnvironmentVariable("TestKey") == "test value";
            valid &= args[0] == "arg1";
            valid &= args[1] == "arg2";
            valid &= args[2] == "arg3";
            return valid ? 0 : 1;

            """,
            TestContext.Current.CancellationToken
        );

        var inputPath = "secrets.json";

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync(
            $"arg1 arg2 arg3 --configuration Release --file {inputPath}",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task RunCommand_MissingSecretKey_OutputSopsError()
    {
        // Arrange
        var command = new RunCommand(_serviceProvider);

        // Sops config
        await File.WriteAllTextAsync(
            ".sops.yaml",
            """
            creation_rules:
              - path_regex: secrets.json
                age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8
            """,
            TestContext.Current.CancellationToken
        );

        // Set SOPS_AGE_KEY_FILE to a empty keys file
        var keysFilePath = Path.Join(
            _uniqueCurrentDirectoryFixture.TestDirectory.FullName,
            "keys.txt"
        );
        await File.Create(keysFilePath).DisposeAsync();
        Environment.SetEnvironmentVariable("SOPS_AGE_KEY_FILE", keysFilePath);

        await File.WriteAllTextAsync(
            "secrets.json",
            /*lang=json,strict*/
            """
            {
                "TestKey": "ENC[AES256_GCM,data:L3MRIGiBVhqFcA==,iv:+57aY2xTo6lwwVaUF2ifbvgs5uPT0xsmwPFlWRexFrg=,tag:b3v7JRAhLxZRCgblwGOZvg==,type:str]",
                "sops": {
                    "kms": null,
                    "gcp_kms": null,
                    "azure_kv": null,
                    "hc_vault": null,
                    "age": [
                        {
                            "recipient": "age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8",
                            "enc": "-----BEGIN AGE ENCRYPTED FILE-----\nYWdlLWVuY3J5cHRpb24ub3JnL3YxCi0+IFgyNTUxOSB1ZkdnUlhidzV1bjdObDE1\nbXZSYVpObExxVnE1WEVrNnN6cDE2WUpUSjNZCmJzYktWRkFhT2RwOFF0N01mMFEv\nNXF3TmVzZE1CczNHblNLNU5CVWVBVE0KLS0tIFNXam5rOVozR3RHaFlCRkJnVEVv\nQ2t6VDNHVUJJTXZTVXVMNzVKaVkyTG8K2owMkncIH4WDJ3IG0v0hO8e9/LLhbFAk\nK4jc3emW/DBRQwvtGkGMOa/WKOV29R3t4TYbaHxeLmVIF1u+43rmfQ==\n-----END AGE ENCRYPTED FILE-----\n"
                        }
                    ],
                    "lastmodified": "2023-07-21T08:21:43Z",
                    "mac": "ENC[AES256_GCM,data:r6KgqW5MSMf7NUAusF8foeIHTd2UkwnACqi5CPZbE28F9qhyWbtlC1NHwThGB0Jd0cOAigHmRLTAjodx6g+sStllqjAVnQQtFY4lyw1jbA1YkDx8ICB7WV3CytJBZJ7ZdQbH8gSqJMScDVAbVQb+UUi5+cipVjpaJiWU4UdNtVI=,iv:Rnr0pqpufMtaYoOe3O7DYsv73Fta2aTb9IHqbap9gFs=,tag:IHUNSDnca6vKeTEDjywb+w==,type:str]",
                    "pgp": null,
                    "unencrypted_suffix": "_unencrypted",
                    "version": "3.7.3"
                }
            }
            """,
            TestContext.Current.CancellationToken
        );

        var inputPath = "secrets.json";

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync(
            $"--file {inputPath}",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(128, exitCode);
        Assert.Equal(
            """
            Executing SOPS failed.

            """,
            _logger.Error.Output,
            ignoreLineEndingDifferences: true
        );
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
            Output = new ReplaceUsageHelpTextWriter(
                output,
                Assembly.GetExecutingAssembly().GetName().Name!
            ),
        };

        // Act
        var exitCode = await config.InvokeAsync(
            $"run {option}",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Equal(
            """
            Description:
              Execute 'dotnet run' with decrypted secrets inserted into the environment

            Usage:
              dotnet sops run [<dotnetArguments>...] [options]

            Arguments:
              <dotnetArguments>  Arguments passed to the 'dotnet run' command.

            Options:
              --file          Encrypted secrets file [default: secrets.json]
              -?, -h, --help  Show help and usage information
              --verbose       Enable verbose logging output


            """,
            output.ToString().RemoveHelpWrapNewLines(),
            ignoreLineEndingDifferences: true
        );
    }
}
