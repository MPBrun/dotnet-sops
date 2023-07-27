using System.CommandLine;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotnetSops.CommandLine.Commands;

internal partial class EncryptCommand : CliCommand
{
    public const string CommandName = "encrypt";

    private readonly Services.IServiceProvider _serviceProvider;

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

    public EncryptCommand(Services.IServiceProvider serviceProvider)
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
                _serviceProvider.AnsiConsoleError.Value,
                _serviceProvider.ProjectInfoService.Value,
                _serviceProvider.SopsService.Value,
                _serviceProvider.UserSecretsService.Value,
                _serviceProvider.FileBomService.Value,
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(FileInfo? projectFile, string? userSecretId, FileInfo outputFile, IAnsiConsole consoleError, IProjectInfoService projectInfoService, ISopsService sopsService, IUserSecretsService userSecretsService, IFileBomService fileBomService, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userSecretId))
        {
            try
            {
                userSecretId = projectInfoService.FindUserSecretId(projectFile);
            }
            catch (ProjectInfoSearchException ex)
            {
                consoleError.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                return 1;
            }
        }

        var inputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);
        if (!inputFile.Exists)
        {
            consoleError.MarkupLine(LocalizationResources.UserSecretsFileDoesNotExist(inputFile.FullName));
            return 1;
        }

        await fileBomService.RemoveBomFromFileAsync(inputFile, cancellationToken);

        try
        {
            await sopsService.EncryptAsync(inputFile, outputFile, cancellationToken);

            consoleError.MarkupLineInterpolated($"[green]User secret with id \"{userSecretId}\" successfully encrypted to \"{outputFile.FullName}\".[/]");

            return 0;
        }
        catch (SopsMissingException ex)
        {
            consoleError.MarkupLine(ex.Message);
            return 1;
        }
        catch (SopsExecutionException ex)
        {
            consoleError.MarkupLine(ex.Message);
            return ex.ExitCode;
        }
    }
}
