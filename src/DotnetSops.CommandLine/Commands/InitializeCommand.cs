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

        SetAction(
            (parseResult, cancellationToken) =>
                ExecuteAsync(_serviceProvider.GetRequiredService<ILogger>(), cancellationToken)
        );
    }

    private static async Task<int> ExecuteAsync(ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation(Properties.Resources.InitializeCommandKeyInformation);
        logger.LogInformation();

        var alreadyExist = File.Exists(".sops.yaml");
        if (alreadyExist)
        {
            var generate = await logger.ConfirmAsync(
                Properties.Resources.InitializeCommandConfigAlreadyExistQuestion,
                cancellationToken
            );
            if (!generate)
            {
                return 0;
            }
        }

        var sopsConfiguration = await PromptSopsConfigurationAsync(logger, cancellationToken);

        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithIndentedSequences()
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

    private static async Task<SopsConfiguration> PromptSopsConfigurationAsync(
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        var sopsCreationRule = new SopsCreationRule() { PathRegex = "secrets.json" };
        var sopsConfiguration = new SopsConfiguration() { CreationRules = [sopsCreationRule] };

        var useKeyGroups = await logger.ConfirmAsync(
            Properties.Resources.InitializeCommandUseKeyGroupsQuestion,
            cancellationToken
        );
        if (useKeyGroups)
        {
            await PromptKeyGroupsKeysAsync(logger, sopsCreationRule, cancellationToken);
        }
        else
        {
            await PromptSimpleKeysAsync(logger, sopsCreationRule, cancellationToken);
        }

        return sopsConfiguration;
    }

    private static async Task PromptKeyGroupsKeysAsync(
        ILogger logger,
        SopsCreationRule sopsCreationRule,
        CancellationToken cancellationToken
    )
    {
        do
        {
            var keyGroup = new SopsKeyGroup();
            do
            {
                var keyType = await PromptKeyTypeAsync(logger, cancellationToken);
                switch (keyType)
                {
                    case SopsKeyType.AzureKeyVault:
                    {
                        logger.LogInformation(
                            Properties.Resources.InitializeCommandAzureKeyVaultFormat
                        );
                        var keyVaultName = await logger.AskAsync(
                            Properties.Resources.InitializeCommandAzureKeyVaultNameQuestion,
                            cancellationToken
                        );
                        var keyName = await logger.AskAsync(
                            Properties.Resources.InitializeCommandAzureKeyVaultKeyQuestion,
                            cancellationToken
                        );
                        var keyVersion = await logger.AskAsync(
                            Properties.Resources.InitializeCommandAzureKeyVaultVersionQuestion,
                            cancellationToken
                        );

                        var azureKeyVaultKey = new SopsAzureKeyVaultKeyGroup()
                        {
                            VaultUrl = $"https://{keyVaultName.Trim()}.vault.azure.net/",
                            Key = keyName.Trim(),
                            Version = keyVersion.Trim(),
                        };

                        keyGroup.AzureKeyvault ??= [];
                        keyGroup.AzureKeyvault.Add(azureKeyVaultKey);
                        break;
                    }
                    case SopsKeyType.AwsKms:
                    {
                        logger.LogInformation(Properties.Resources.InitializeCommandAwsKmsFormat);
                        var arn = await logger.AskAsync(
                            Properties.Resources.InitializeCommandAwsKmsQuestion,
                            cancellationToken
                        );
                        keyGroup.Kms ??= [];
                        keyGroup.Kms.AddRange(
                            arn.Trim().Split(",").Select(id => new SopsKmsKeyGroup() { Arn = id })
                        );
                        break;
                    }
                    case SopsKeyType.GcpKms:
                    {
                        logger.LogInformation(Properties.Resources.InitializeCommandGcpKmsFormat);
                        var resourceId = await logger.AskAsync(
                            Properties.Resources.InitializeCommandGcpKmsQuestion,
                            cancellationToken
                        );
                        keyGroup.GcpKms ??= [];
                        keyGroup.GcpKms.AddRange(
                            resourceId
                                .Trim()
                                .Split(",")
                                .Select(id => new SopsGcpKmsKeyGroup() { ResourceId = id })
                        );
                        break;
                    }
                    case SopsKeyType.HashicorpVault:
                    {
                        var url = await logger.AskAsync(
                            Properties.Resources.InitializeCommandHashicorpVaultQuestion,
                            cancellationToken
                        );
                        keyGroup.Vault ??= [];
                        keyGroup.Vault.AddRange(url.Trim().Split(","));
                        break;
                    }
                    case SopsKeyType.Age:
                    {
                        var publicKey = await logger.AskAsync(
                            Properties.Resources.InitializeCommandAgePublicKeyQuestion,
                            cancellationToken
                        );
                        keyGroup.Age ??= [];
                        keyGroup.Age.AddRange(publicKey.Trim().Split(","));
                        break;
                    }

                    case SopsKeyType.Pgp:
                    {
                        var publicKey = await logger.AskAsync(
                            Properties.Resources.InitializeCommandPgpPublicKeyQuestion,
                            cancellationToken
                        );
                        keyGroup.Pgp ??= [];
                        keyGroup.Pgp.AddRange(publicKey.Trim().Split(","));
                        break;
                    }
                    default:
                        break;
                }
            } while (
                await logger.ConfirmAsync(
                    Properties.Resources.InitializeCommandMoreKeysToKeyGroupQuestion,
                    cancellationToken
                )
            );

            sopsCreationRule.KeyGroups ??= [];
            sopsCreationRule.KeyGroups.Add(keyGroup);
        } while (
            await logger.ConfirmAsync(
                Properties.Resources.InitializeCommandMoreKeyGroupsQuestion,
                cancellationToken
            )
        );
    }

    private static async Task PromptSimpleKeysAsync(
        ILogger logger,
        SopsCreationRule sopsCreationRule,
        CancellationToken cancellationToken
    )
    {
        do
        {
            var keyType = await PromptKeyTypeAsync(logger, cancellationToken);
            switch (keyType)
            {
                case SopsKeyType.AzureKeyVault:
                {
                    logger.LogInformation(
                        Properties.Resources.InitializeCommandAzureKeyVaultIdentifierFormat
                    );
                    var keyIdentifier = await logger.AskAsync(
                        Properties.Resources.InitializeCommandAzureKeyIdentifierQuestion,
                        cancellationToken
                    );

                    sopsCreationRule.AzureKeyvault = (
                        sopsCreationRule.AzureKeyvault + "," + keyIdentifier
                    ).Trim(' ', ',');
                    break;
                }
                case SopsKeyType.AwsKms:
                {
                    logger.LogInformation(Properties.Resources.InitializeCommandAwsKmsFormat);
                    var arn = await logger.AskAsync(
                        Properties.Resources.InitializeCommandAwsKmsQuestion,
                        cancellationToken
                    );

                    sopsCreationRule.Kms = (sopsCreationRule.Kms + "," + arn).Trim(' ', ',');

                    break;
                }
                case SopsKeyType.GcpKms:
                {
                    logger.LogInformation(Properties.Resources.InitializeCommandGcpKmsFormat);
                    var resourceId = await logger.AskAsync(
                        Properties.Resources.InitializeCommandGcpKmsQuestion,
                        cancellationToken
                    );

                    sopsCreationRule.GcpKms = (sopsCreationRule.GcpKms + "," + resourceId).Trim(
                        ' ',
                        ','
                    );
                    break;
                }
                case SopsKeyType.HashicorpVault:
                {
                    var url = await logger.AskAsync(
                        Properties.Resources.InitializeCommandHashicorpVaultQuestion,
                        cancellationToken
                    );

                    sopsCreationRule.HcVaultTransitUri = (
                        sopsCreationRule.HcVaultTransitUri + "," + url
                    ).Trim(' ', ',');
                    break;
                }
                case SopsKeyType.Age:
                {
                    var publicKey = await logger.AskAsync(
                        Properties.Resources.InitializeCommandAgePublicKeyQuestion,
                        cancellationToken
                    );

                    sopsCreationRule.Age = (sopsCreationRule.Age + "," + publicKey).Trim(' ', ',');
                    break;
                }

                case SopsKeyType.Pgp:
                {
                    var publicKey = await logger.AskAsync(
                        Properties.Resources.InitializeCommandPgpPublicKeyQuestion,
                        cancellationToken
                    );

                    sopsCreationRule.Pgp = (sopsCreationRule.Pgp + "," + publicKey).Trim(' ', ',');
                    break;
                }
                default:
                    break;
            }
        } while (
            await logger.ConfirmAsync(
                Properties.Resources.InitializeCommandMoreKeysQuestion,
                cancellationToken
            )
        );
    }

    private static async Task<SopsKeyType> PromptKeyTypeAsync(
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        static string KeyTypeConverter(SopsKeyType keyType)
        {
            return keyType switch
            {
                SopsKeyType.AzureKeyVault => Properties.Resources.KeyTypeAzureKeyVault,
                SopsKeyType.AwsKms => Properties.Resources.KeyTypeAwsKms,
                SopsKeyType.GcpKms => Properties.Resources.KeyTypeGcpKms,
                SopsKeyType.HashicorpVault => Properties.Resources.KeyTypeHashicorpVault,
                SopsKeyType.Age => Properties.Resources.KeyTypeAge,
                SopsKeyType.Pgp => Properties.Resources.KeyTypePgp,
                _ => throw new NotSupportedException(),
            };
        }

        var keyChoices = new SopsKeyType[]
        {
            SopsKeyType.AzureKeyVault,
            SopsKeyType.AwsKms,
            SopsKeyType.GcpKms,
            SopsKeyType.HashicorpVault,
            SopsKeyType.Age,
            SopsKeyType.Pgp,
        };

        return await logger.SelectAsync(
            Properties.Resources.InitializeCommandKeyTypeQuestion,
            keyChoices,
            KeyTypeConverter,
            cancellationToken
        );
    }
}
