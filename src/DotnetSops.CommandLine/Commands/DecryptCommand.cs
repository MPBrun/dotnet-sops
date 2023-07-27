using System.CommandLine;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotnetSops.CommandLine.Commands;

internal class DecryptCommand : CliCommand
{
    public const string CommandName = "decrypt";

    private readonly Services.IServiceProvider _serviceProvider;

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
        DefaultValueFactory = _ => new FileInfo("secrets.json")
    };

    public DecryptCommand(Services.IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.DecryptCommandDescription)
    {
        Add(_projectFileOption);
        Add(_userSecretsIdOption);
        Add(_inputFileOption);

        _serviceProvider = serviceProvider;

        SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
                parseResult.GetValue(_projectFileOption),
                parseResult.GetValue(_userSecretsIdOption),
                parseResult.GetValue(_inputFileOption)!,
                _serviceProvider.AnsiConsoleError.Value,
                _serviceProvider.ProjectInfoService.Value,
                _serviceProvider.SopsService.Value,
                _serviceProvider.UserSecretsService.Value,
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(FileInfo? projectFile, string? userSecretId, FileInfo inputFile, IAnsiConsole consoleError, IProjectInfoService projectInfoService, ISopsService sopsService, IUserSecretsService userSecretsService, CancellationToken cancellationToken)
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

        if (!inputFile.Exists)
        {
            consoleError.MarkupLine(LocalizationResources.FileDoesNotExist(inputFile.FullName));
            return 1;
        }

        var outputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);

        if (outputFile.Directory != null)
        {
            Directory.CreateDirectory(outputFile.Directory.FullName);
        }

        try
        {
            await sopsService.DecryptAsync(inputFile, outputFile, cancellationToken);

            consoleError.MarkupLineInterpolated($"[green]{inputFile.Name} successfully decrypted to user secret with id \"{userSecretId}\".[/]");

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
