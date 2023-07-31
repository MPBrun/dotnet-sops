using System.CommandLine;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSops.CommandLine.Commands;

internal class EncryptCommand : CliCommand
{
    public const string CommandName = "encrypt";

    private readonly IServiceProvider _serviceProvider;

    private readonly CliOption<FileInfo?> _projectFileOption = new("--project", "-p")
    {
        Description = Properties.Resources.EncryptCommandProjectOptionDescription,
    };

    private readonly CliOption<string> _userSecretsIdOption = new("--id")
    {
        Description = Properties.Resources.EncryptCommandSecretsIdOptionDescription,
    };

    private readonly CliOption<FileInfo> _outputFileOption = new("--file")
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

        SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
                parseResult.GetValue(_projectFileOption),
                parseResult.GetValue(_userSecretsIdOption),
                parseResult.GetValue(_outputFileOption)!,
                _serviceProvider.GetRequiredService<ILogger>(),
                _serviceProvider.GetRequiredService<IProjectInfoService>(),
                _serviceProvider.GetRequiredService<ISopsService>(),
                _serviceProvider.GetRequiredService<IUserSecretsService>(),
                _serviceProvider.GetRequiredService<IFileBomService>(),
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(FileInfo? projectFile, string? userSecretId, FileInfo outputFile, ILogger logger, IProjectInfoService projectInfoService, ISopsService sopsService, IUserSecretsService userSecretsService, IFileBomService fileBomService, CancellationToken cancellationToken)
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
            logger.LogInformation($"""
                                You have no secrets created. You can add secrets by running this command:
                                  [yellow]dotnet user-secrets set [[name]] [[value]][/]
                                """);
            return 1;
        }

        await fileBomService.RemoveBomFromFileAsync(inputFile, cancellationToken);

        try
        {
            await sopsService.EncryptAsync(inputFile, outputFile, cancellationToken);

            logger.LogSuccess($"User secret with id '{userSecretId}' successfully encrypted to '{outputFile.FullName}'.");

            return 0;
        }
        catch (SopsMissingException ex)
        {
            logger.LogError(ex.Message);
            logger.LogInformation();
            logger.LogInformation(Properties.Resources.SopsIsMissingTry);
            return 1;
        }
        catch (SopsExecutionException ex)
        {
            logger.LogError(ex.Message);
            return ex.ExitCode;
        }
    }
}
