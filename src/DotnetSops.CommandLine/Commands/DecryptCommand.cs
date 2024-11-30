using System.CommandLine;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSops.CommandLine.Commands;

internal class DecryptCommand : CliCommand
{
    public const string CommandName = "decrypt";

    private readonly IServiceProvider _serviceProvider;

    private readonly CliOption<FileInfo?> _projectFileOption = new("--project", "-p")
    {
        Description = Properties.Resources.DecryptCommandProjectOptionDescription,
    };

    private readonly CliOption<string?> _userSecretsIdOption = new("--id")
    {
        Description = Properties.Resources.DecryptCommandSecretsIdOptionDescription,
    };

    private readonly CliOption<FileInfo> _inputFileOption = new("--file")
    {
        Description = Properties.Resources.DecryptCommandFileOptionDescription,
        DefaultValueFactory = _ => new FileInfo("secrets.json"),
    };

    public DecryptCommand(IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.DecryptCommandDescription)
    {
        Add(_projectFileOption);
        Add(_userSecretsIdOption);
        Add(_inputFileOption);

        _serviceProvider = serviceProvider;

        SetAction(
            (parseResult, cancellationToken) =>
                ExecuteAsync(
                    parseResult.GetValue(_projectFileOption),
                    parseResult.GetValue(_userSecretsIdOption),
                    parseResult.GetValue(_inputFileOption)!,
                    _serviceProvider.GetRequiredService<ILogger>(),
                    _serviceProvider.GetRequiredService<IProjectInfoService>(),
                    _serviceProvider.GetRequiredService<ISopsService>(),
                    _serviceProvider.GetRequiredService<IUserSecretsService>(),
                    cancellationToken
                )
        );
    }

    private static async Task<int> ExecuteAsync(
        FileInfo? projectFile,
        string? userSecretId,
        FileInfo inputFile,
        ILogger logger,
        IProjectInfoService projectInfoService,
        ISopsService sopsService,
        IUserSecretsService userSecretsService,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug($"Using input file '{inputFile}'");

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

        var outputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);
        logger.LogDebug($"Using user secret file '{outputFile}'");

        if (!inputFile.Exists)
        {
            logger.LogError(LocalizationResources.FileDoesNotExist(inputFile.FullName));
            return 1;
        }

        if (outputFile.Directory != null)
        {
            _ = Directory.CreateDirectory(outputFile.Directory.FullName);
        }

        try
        {
            await sopsService.DecryptAsync(inputFile, outputFile, cancellationToken);

            logger.LogSuccess(
                LocalizationResources.DecryptCommandSuccess(inputFile.Name, userSecretId)
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
