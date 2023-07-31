using System.CommandLine;
using DotnetSops.CommandLine.Commands.Prompts;
using DotnetSops.CommandLine.Services.Sops;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotnetSops.CommandLine.Commands;
internal class InitializeCommand : CliCommand
{
    public const string CommandName = "init";

    private readonly Services.IServiceProvider _serviceProvider;

    public InitializeCommand(Services.IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.InitializeCommandDescription)
    {
        _serviceProvider = serviceProvider;

        SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
                _serviceProvider.AnsiConsoleError.Value,
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(IAnsiConsole consoleError, CancellationToken cancellationToken)
    {
        var alreadyExist = File.Exists(".sops.yaml");
        if (alreadyExist)
        {
            var generate = await new Prompts.ConfirmationPrompt("[green]?[/] Are you sure to overwrite existing [yellow].sops.yaml[/]?")
                .ShowAsync(consoleError, cancellationToken);
            if (!generate)
            {
                return 0;
            }
        }

        var encryptionTypeConverter = (SopsEncryptionType encryptionType) =>
        {
            return encryptionType switch
            {
                SopsEncryptionType.AzureKeyVault => "Azure Key Vault",
                SopsEncryptionType.AwsKms => "AWS KMS",
                SopsEncryptionType.GcpKms => "GCP KMS",
                SopsEncryptionType.HashicorpVault => "Hashicorp Vault",
                SopsEncryptionType.Age => "age",
                SopsEncryptionType.Pgp => "PGP",
                _ => throw new NotSupportedException(),
            };
        };

        var encryptionType = await new SelectionPrompt<SopsEncryptionType>()
            .Title("[green]?[/] Which encryption whould you like to use?")
            .UseConverter(encryptionTypeConverter)
            .WrapAround(true)
            .AddChoices(
                SopsEncryptionType.AzureKeyVault,
                SopsEncryptionType.AwsKms,
                SopsEncryptionType.GcpKms,
                SopsEncryptionType.HashicorpVault,
                SopsEncryptionType.Age,
                SopsEncryptionType.Pgp)
            .ShowAsync(consoleError, cancellationToken);

        consoleError.MarkupLineInterpolated($"[green]?[/] Which encryption whould you like to use? [blue]{encryptionTypeConverter(encryptionType)}[/]");

        var sopsCreationRule = new SopsCreationRule()
        {
            PathRegex = "secrets.json",
        };
        var sopsConfiguration = new SopsConfiguration()
        {
            CreationRules = new List<SopsCreationRule>()
            {
                sopsCreationRule
            }
        };

        switch (encryptionType)
        {
            case SopsEncryptionType.AzureKeyVault:
                {
                    var keyVaultName = await new AskPrompt("[green]?[/] What is the name of the key vault?")
                        .ShowAsync(consoleError, cancellationToken);
                    var keyName = await new AskPrompt("[green]?[/] What is object name of the key?")
                        .ShowAsync(consoleError, cancellationToken);
                    var keyVersion = await new AskPrompt("[green]?[/] What is object version of the key?")
                        .ShowAsync(consoleError, cancellationToken);

                    var key = $"https://{keyVaultName.Trim()}.vault.azure.net/keys/{keyName.Trim()}/{keyVersion.Trim()}";
                    sopsCreationRule.AzureKeyvault = key;
                    break;
                }
            case SopsEncryptionType.AwsKms:
                throw new NotImplementedException();
            case SopsEncryptionType.GcpKms:
                throw new NotImplementedException();
            case SopsEncryptionType.HashicorpVault:
                throw new NotImplementedException();
            case SopsEncryptionType.Age:
                {
                    var publicKey = await new AskPrompt("[green]?[/] What is public key of age?")
                        .ShowAsync(consoleError, cancellationToken);
                    sopsCreationRule.Age = publicKey.Trim();
                    break;
                }

            case SopsEncryptionType.Pgp:
                {
                    var publicKey = await new AskPrompt("[green]?[/] What is public key of PGP?")
                        .ShowAsync(consoleError, cancellationToken);
                    sopsCreationRule.Pgp = publicKey.Trim();
                    break;
                }
            default:
                break;
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        var content = serializer.Serialize(sopsConfiguration);

        await File.WriteAllTextAsync(".sops.yaml", content, cancellationToken);

        consoleError.WriteLine();
        consoleError.MarkupLine("[green]Generated .sops.yaml with the following content:[/]");
        consoleError.MarkupLineInterpolated($"[gray]{content}[/]");

        consoleError.MarkupLine("You can now encrypt your dotnet user secrets by running:");
        consoleError.MarkupLineInterpolated($"  [yellow]dotnet sops encrypt[/]");

        return 0;
    }
}
