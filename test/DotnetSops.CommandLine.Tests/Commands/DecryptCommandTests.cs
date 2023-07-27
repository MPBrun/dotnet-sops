using System.CommandLine;
using System.Text.Json;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using DotnetSops.CommandLine.Tests.Fixtures;
using DotnetSops.CommandLine.Tests.Models;
using DotnetSops.CommandLine.Tests.Services;
using DotnetSops.CommandLine.Tests.Services.UserSecrets;
using Spectre.Console;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Commands;

[Collection(CollectionNames.Sops)]
public class DecryptCommandTests
{
    public DecryptCommandTests()
    {
        Environment.SetEnvironmentVariable("SOPS_AGE_KEY", null);
    }

    [Fact]
    public async Task DecryptCommand_ValidOptions_CreateFile()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var projectInfoService = new ProjectInfoService();
        var sopsService = new SopsService(dir.FullName);
        var userSecretsService = new UserSecretsServiceStub(dir.FullName);
        var fileBomService = new FileBomService();
        using var ansiConsole = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            ProjectInfoService = new Lazy<IProjectInfoService>(projectInfoService),
            SopsService = new Lazy<ISopsService>(sopsService),
            UserSecretsService = new Lazy<IUserSecretsService>(userSecretsService),
            FileBomService = new Lazy<IFileBomService>(fileBomService),
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new DecryptCommand(serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = userSecretsService.GetSecretsPathFromSecretsId(id);

        // Provide age secret key for unit test purpose
        Environment.SetEnvironmentVariable("SOPS_AGE_KEY", "AGE-SECRET-KEY-10HA9FMZENQKN8DXGZPRWZ7YK5R83AYK4FQVZ8Y5LPAV3430HXW7QZAFV9Z");

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  age: 'age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8'
            """);

        var secretPath = Path.Combine(dir.FullName, "secrets.json");
        await File.WriteAllTextAsync(secretPath, /*lang=json,strict*/ """
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
            """);

        var inputPath = Path.Combine(dir.FullName, "secrets.json");

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {inputPath}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.True(File.Exists(filePath.FullName));

        var secretContent = JsonSerializer.Deserialize<TestSecretCotent>(File.ReadAllText(filePath.FullName))!;
        Assert.Equal("test value", secretContent.TestKey);
    }

    [Fact]
    public async Task DecryptCommand_MissingSecretKey_OutputSopsError()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var projectInfoService = new ProjectInfoService();
        var sopsService = new SopsService(dir.FullName);
        var userSecretsService = new UserSecretsServiceStub(dir.FullName);
        var fileBomService = new FileBomService();
        using var ansiConsole = new TestConsole();

        var serviceProvider = new MockServiceProvider()
        {
            ProjectInfoService = new Lazy<IProjectInfoService>(projectInfoService),
            SopsService = new Lazy<ISopsService>(sopsService),
            UserSecretsService = new Lazy<IUserSecretsService>(userSecretsService),
            FileBomService = new Lazy<IFileBomService>(fileBomService),
            AnsiConsoleError = new Lazy<IAnsiConsole>(ansiConsole),
        };

        var command = new DecryptCommand(serviceProvider);
        var id = $"unittest-{Guid.NewGuid()}";

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: secrets.json
                  age: 'age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8'
            """);

        var secretPath = Path.Combine(dir.FullName, "secrets.json");
        await File.WriteAllTextAsync(secretPath, /*lang=json,strict*/ """
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
            """);

        var inputPath = Path.Combine(dir.FullName, "secrets.json");

        var config = new CliConfiguration(command);

        // Act
        var exitCode = await config.InvokeAsync($"--id {id} --file {inputPath}");

        // Assert
        Assert.Equal(128, exitCode);
        Assert.Equal("""
            SOPS failed with error.

            Failed to get the data key required to decrypt the SOPS file.

            Group 0: FAILED
              age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8: FAILED
                - | no secret keys found

            Recovery failed because no master key was able to decrypt the file. In
            order for SOPS to recover the file, at least one key has to be successful,
            but none were.

            """, ansiConsole.Output, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void DecryptCommand_Id_NotRequired()
    {
        // Arrange
        var command = new DecryptCommand(new MockServiceProvider());

        // Act / Assert
        var option = command.Options.First(o => o.Name == "--id");
        Assert.False(option.Required);
    }

    [Fact]
    public void DecryptCommand_File_NotRequired()
    {
        // Arrange
        var command = new DecryptCommand(new MockServiceProvider());

        // Act / Assert
        var option = command.Options.First(o => o.Name == "--file");
        Assert.False(option.Required);
    }
}
