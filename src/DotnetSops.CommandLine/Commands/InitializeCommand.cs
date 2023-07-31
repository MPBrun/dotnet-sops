using System.CommandLine;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotnetSops.CommandLine.Commands;
internal class InitializeCommand : CliCommand
{
    public const string CommandName = "init";

    private readonly IServiceProvider _serviceProvider;

    public InitializeCommand(IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.InitializeCommandDescription)
    {
        _serviceProvider = serviceProvider;

        SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
                _serviceProvider.GetRequiredService<ILogger>(),
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(ILogger logger, CancellationToken cancellationToken)
    {
        var alreadyExist = File.Exists(".sops.yaml");
        if (alreadyExist)
        {
            var generate = await logger.ConfirmAsync(Properties.Resources.InitializeCommandConfigAlreadyExistQuestion, cancellationToken);
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

        var choices = new SopsEncryptionType[]
        {
            SopsEncryptionType.AzureKeyVault,
            SopsEncryptionType.AwsKms,
            SopsEncryptionType.GcpKms,
            SopsEncryptionType.HashicorpVault,
            SopsEncryptionType.Age,
            SopsEncryptionType.Pgp
        };

        var encryptionType = await logger.SelectAsync(Properties.Resources.InitializeCommandEncryptionQuestion, choices, encryptionTypeConverter, cancellationToken);

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
                    var keyVaultName = await logger.AskAsync(Properties.Resources.InitializeCommandAzureKeyVaultNameQuestion, cancellationToken);
                    var keyName = await logger.AskAsync(Properties.Resources.InitializeCommandAzureKeyVaultKeyQuestion, cancellationToken);
                    var keyVersion = await logger.AskAsync(Properties.Resources.InitializeCommandAzureKeyVaultVersionQuestion, cancellationToken);

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
                    var publicKey = await logger.AskAsync(Properties.Resources.InitializeCommandAgePublicKeyQuestion, cancellationToken);
                    sopsCreationRule.Age = publicKey.Trim();
                    break;
                }

            case SopsEncryptionType.Pgp:
                {
                    var publicKey = await logger.AskAsync(Properties.Resources.InitializeCommandPgpPublicKeyQuestion, cancellationToken);
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

        logger.LogInformation();
        logger.LogInformation(Properties.Resources.InitializeCommandSuccessGenerated);
        logger.LogInformationInterpolated($"[gray]{content}[/]");

        logger.LogInformation(Properties.Resources.InitializeCommandSuccessSuggestion);

        return 0;
    }
}
