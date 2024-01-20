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
    public async Task InitializeComamnd_NoOverwrite_Stop()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        await File.WriteAllTextAsync(".sops.yaml", "");

        // Don't overwrite existing file
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Equal(
            """
            Keys:
              SOPS supports different keys. You can refer to their respective documentation on how to create a key that supports encryption and decryption.

            Documentation:
              Azure Key Vault - https://learn.microsoft.com/en-us/azure/key-vault
              AWS KMS - https://aws.amazon.com/kms
              GCP KMS - https://cloud.google.com/security-key-management
              Hashicorp Vault - https://www.vaultproject.io
              age - https://github.com/FiloSottile/age
              PGP - https://www.openpgp.org

            Key groups:
              SOPS can be used to encrypt data under multiple keys, so that if any of the keys are available, the data can be decrypted. 
              
              However, SOPS also supports "key groups", which require access to multiple keys in order to decrypt.


            ? Are you sure you want to overwrite the existing .sops.yaml? [y/n] (n): n

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            ignoreLineEndingDifferences: true
        );
    }

    [Fact]
    public async Task InitializeCommand_AzureKeyVault_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // No key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Select Azure Key Vault
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter(
            "https://vault-name.vault.azure.net/keys/name/version"
        );

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): n\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            """
            ? Which key type would you like to use? Azure Key Vault
            A key identifier has following format: https://{vault-name}.vault.azure.net/keys/{object-name}/{object-version}
            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the key identifier of the key? https://vault-name.vault.azure.net/keys/name/version",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                azure_keyvault: https://vault-name.vault.azure.net/keys/name/version

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                azure_keyvault: https://vault-name.vault.azure.net/keys/name/version

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_AwsKms_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // No key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Select AWS KMS
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter(
            "arn:example:service-1:region-eu:account-id:resource-type/resource-id"
        );

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): n\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use? AWS KMS",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            """
            ? Which key type would you like to use? AWS KMS
            An ARN has following format: arn:partition:service:region:account-id:resource-type/resource-id
            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the ARN of the key? arn:example:service-1:region-eu:account-id:resource-type/resource-id",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                kms: arn:example:service-1:region-eu:account-id:resource-type/resource-id

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                kms: arn:example:service-1:region-eu:account-id:resource-type/resource-id

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_GcpKms_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // No key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Select GCP KMS
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter(
            "projects/foo/locations/eu/keyRings/ring-1/cryptoKeys/bar"
        );

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): n\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use? GCP KMS",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            """
            ? Which key type would you like to use? GCP KMS
            The resource ID has the following format: projects/PROJECT_ID/locations/LOCATION/keyRings/KEY_RING/cryptoKeys/KEY_NAME
            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the resource ID of the key? projects/foo/locations/eu/keyRings/ring-1/cryptoKeys/bar",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                gcp_kms: projects/foo/locations/eu/keyRings/ring-1/cryptoKeys/bar

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                gcp_kms: projects/foo/locations/eu/keyRings/ring-1/cryptoKeys/bar

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_HashicorpVault_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // No key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Select Hashicorp Vault
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter("http://127.0.0.1/vault/key");

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): n\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use? Hashicorp Vault",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            """
            ? Which key type would you like to use? Hashicorp Vault
            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the URI of the vault key? http://127.0.0.1/vault/key",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                hc_vault_transit_uri: http://127.0.0.1/vault/key

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                hc_vault_transit_uri: http://127.0.0.1/vault/key

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_Age_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        await File.WriteAllTextAsync(".sops.yaml", "");

        // Owerwrite existing file
        _logger.Error.Input.PushTextWithEnter("y");

        // No key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Select age
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter(
            "age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8"
        );

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Are you sure you want to overwrite the existing .sops.yaml? [y/n] (n): y",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Use key groups? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the public key of age? age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                age: age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_Pgp_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        await File.WriteAllTextAsync(".sops.yaml", "");

        // Owerwrite existing file
        _logger.Error.Input.PushTextWithEnter("y");

        // No key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Select pgp
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter("123123");

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Are you sure you want to overwrite the existing .sops.yaml? [y/n] (n): y",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Use key groups? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the public key of PGP? 123123",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                pgp: 123123

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                pgp: 123123

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_KeyGroupAzureKeyVault_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // key groups
        _logger.Error.Input.PushTextWithEnter("y");

        // Select Azure Key Vault
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter("vault-name");
        _logger.Error.Input.PushTextWithEnter("name");
        _logger.Error.Input.PushTextWithEnter("version");

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // No more key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): y\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            """
            ? Which key type would you like to use? Azure Key Vault
            An Azure Vault key is defined by 3 parts of the key identifier: https://{vault-name}.vault.azure.net/keys/{object-name}/{object-version}
            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the name of the Key Vault? vault-name",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the object name of the key? name",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the object version of the key? version",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys to the group? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more key groups? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - azure_keyvault:
                      - vaultUrl: https://vault-name.vault.azure.net/
                        key: name
                        version: version

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - azure_keyvault:
                      - vaultUrl: https://vault-name.vault.azure.net/
                        key: name
                        version: version

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_KeyGroupAwsKms_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // Key groups
        _logger.Error.Input.PushTextWithEnter("y");

        // Select AWS KMS
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter(
            "arn:example:service-1:region-eu:account-id:resource-type/resource-id"
        );

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // No more key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): y\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use? AWS KMS",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            """
            ? Which key type would you like to use? AWS KMS
            An ARN has following format: arn:partition:service:region:account-id:resource-type/resource-id
            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the ARN of the key? arn:example:service-1:region-eu:account-id:resource-type/resource-id",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys to the group? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more key groups? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - kms:
                      - arn: arn:example:service-1:region-eu:account-id:resource-type/resource-id

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - kms:
                      - arn: arn:example:service-1:region-eu:account-id:resource-type/resource-id

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_KeyGroupGcpKms_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // Key groups
        _logger.Error.Input.PushTextWithEnter("y");

        // Select GCP KMS
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter(
            "projects/foo/locations/eu/keyRings/ring-1/cryptoKeys/bar"
        );

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // No more key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): y\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use? GCP KMS",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            """
            ? Which key type would you like to use? GCP KMS
            The resource ID has the following format: projects/PROJECT_ID/locations/LOCATION/keyRings/KEY_RING/cryptoKeys/KEY_NAME
            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the resource ID of the key? projects/foo/locations/eu/keyRings/ring-1/cryptoKeys/bar",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys to the group? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more key groups? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - gcp_kms:
                      - resource_id: projects/foo/locations/eu/keyRings/ring-1/cryptoKeys/bar

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - gcp_kms:
                      - resource_id: projects/foo/locations/eu/keyRings/ring-1/cryptoKeys/bar

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_KeyGroupHashicorpVault_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // Key groups
        _logger.Error.Input.PushTextWithEnter("y");

        // Select Hashicorp Vault
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter("http://127.0.0.1/vault/key");

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // No more key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): y\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?\n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use? Hashicorp Vault",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            """
            ? Which key type would you like to use? Hashicorp Vault
            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the URI of the vault key? http://127.0.0.1/vault/key",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys to the group? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more key groups? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - vault:
                      - http://127.0.0.1/vault/key

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - vault:
                      - http://127.0.0.1/vault/key

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_KeyGroupAge_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // Key groups
        _logger.Error.Input.PushTextWithEnter("y");

        // Select age
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter(
            "age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8"
        );

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // No more key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): y",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the public key of age? age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys to the group? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more key groups? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - age:
                      - age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - age:
                      - age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8

            """,
            sopsYaml
        );
    }

    [Fact]
    public async Task InitializeCommand_KeyGroupPgp_CreateFile()
    {
        // Arrange
        var command = new InitializeCommand(_serviceProvider);

        var config = new CliConfiguration(command);

        // Key groups
        _logger.Error.Input.PushTextWithEnter("y");

        // Select pgp
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.DownArrow);
        _logger.Error.Input.PushKey(ConsoleKey.Enter);

        // Age public key
        _logger.Error.Input.PushTextWithEnter("123123");

        // No more keys
        _logger.Error.Input.PushTextWithEnter("n");

        // No more key groups
        _logger.Error.Input.PushTextWithEnter("n");

        // Act
        var exitCode = await config.InvokeAsync("");

        // Assert
        Assert.Equal(0, exitCode);

        Assert.Contains(
            "? Use key groups? [y/n] (n): y",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Which key type would you like to use?",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? What is the public key of PGP? 123123",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more keys to the group? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.Contains(
            "? Add more key groups? [y/n] (n): n",
            _logger.Error.Output,
            StringComparison.InvariantCulture
        );
        Assert.EndsWith(
            """

            Generated .sops.yaml with the following content:
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - pgp:
                      - 123123

            You can now encrypt your .NET User Secrets by running:
              dotnet sops encrypt

            """,
            _logger.Error.Output.ReplaceLineEndings(),
            StringComparison.InvariantCulture
        );

        var sopsYaml = await File.ReadAllTextAsync(".sops.yaml");
        Assert.Equal(
            """
            creation_rules:
              - path_regex: secrets.json
                key_groups:
                  - pgp:
                      - 123123

            """,
            sopsYaml
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
            Output = new ReplaceUsageHelpTextWriter(output, "testhost")
        };

        // Act
        var exitCode = await config.InvokeAsync($"init {option}");

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Equal(
            """
            Description:
              Create a .sops.yaml configuration file.

            Usage:
              dotnet sops init [options]

            Options:
              -?, -h, --help  Show help and usage information
              --verbose       Enable verbose logging output


            """,
            output.ToString().RemoveHelpWrapNewLines(),
            ignoreLineEndingDifferences: true
        );
    }
}
