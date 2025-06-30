using System.CommandLine;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSops.CommandLine.Commands;

internal class EncryptCommand : Command
{
    public const string CommandName = "encrypt";

    private readonly IServiceProvider _serviceProvider;

    private readonly Option<FileInfo?> _projectFileOption = new("--project", "-p")
    {
        Description = Properties.Resources.EncryptCommandProjectOptionDescription,
    };

    private readonly Option<string> _userSecretsIdOption = new("--id")
    {
        Description = Properties.Resources.EncryptCommandSecretsIdOptionDescription,
    };

    private readonly Option<FileInfo> _outputFileOption = new("--file")
    {
        Description = Properties.Resources.EncryptCommandFileOptionDescription,
        DefaultValueFactory = (_) => new FileInfo("secrets.json"),
    };

    public EncryptCommand(IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.EncryptCommandDescription)
    {
        _serviceProvider = serviceProvider;

        Add(_projectFileOption);
        Add(_userSecretsIdOption);
        Add(_outputFileOption);

        SetAction(
            (parseResult, cancellationToken) =>
                ExecuteAsync(
                    parseResult.GetValue(_projectFileOption),
                    parseResult.GetValue(_userSecretsIdOption),
                    parseResult.GetValue(_outputFileOption)!,
                    _serviceProvider.GetRequiredService<ILogger>(),
                    _serviceProvider.GetRequiredService<IProjectInfoService>(),
                    _serviceProvider.GetRequiredService<ISopsService>(),
                    _serviceProvider.GetRequiredService<IUserSecretsService>(),
                    _serviceProvider.GetRequiredService<IFileBomService>(),
                    cancellationToken
                )
        );
    }

    private static async Task<int> ExecuteAsync(
        FileInfo? projectFile,
        string? userSecretId,
        FileInfo outputFile,
        ILogger logger,
        IProjectInfoService projectInfoService,
        ISopsService sopsService,
        IUserSecretsService userSecretsService,
        IFileBomService fileBomService,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug($"Using output file '{outputFile}'");

        if (string.IsNullOrWhiteSpace(userSecretId))
        {
            try
            {
                userSecretId = projectInfoService.FindUserSecretId(projectFile);

                logger.LogDebug($"Using usersecret id '{userSecretId}'");
            }
            catch (ProjectInfoSearchException ex)
            {
                logger.LogError(ex.Message);
                if (ex.Suggestion != null)
                {
                    logger.LogInformation();
                    logger.LogInformation(ex.Suggestion);
                }

                return 1;
            }
        }

        var inputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);
        logger.LogDebug($"Using user secret file '{inputFile}'");

        if (!inputFile.Exists)
        {
            logger.LogError(LocalizationResources.UserSecretsFileDoesNotExist(inputFile.FullName));
            logger.LogInformation();
            logger.LogInformation(Properties.Resources.UserSecretsFileDoesNotExistSuggestion);
            return 1;
        }

        await fileBomService.RemoveBomFromFileAsync(inputFile, cancellationToken);

        try
        {
            await sopsService.EncryptAsync(inputFile, outputFile, cancellationToken);

            logger.LogSuccess(
                LocalizationResources.EncryptCommandSuccess(userSecretId, outputFile.FullName)
            );

            return 0;
        }
        catch (SopsMissingException ex)
        {
            logger.LogError(ex.Message);
            logger.LogInformation();
            logger.LogInformation(Properties.Resources.SopsIsMissingSuggestion);
            return 1;
        }
        catch (SopsExecutionException ex)
        {
            logger.LogError(ex.Message);
            return ex.ExitCode;
        }
    }
}
