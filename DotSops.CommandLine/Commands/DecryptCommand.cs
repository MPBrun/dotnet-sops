using System.CommandLine;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotSops.CommandLine.Commands;

internal class DecryptCommand : CliCommand
{
    public const string CommandName = "decrypt";

    private readonly Services.IServiceProvider _serviceProvider;
    private readonly CliOption<string> _userSecretsIdOption = new("--id")
    {
        Description = Properties.Resources.DecryptCommandSecretsIdOptionDescription,
        Required = true
    };
    private readonly CliOption<FileInfo> _inputFileOption = new("--file")
    {
        Description = Properties.Resources.DecryptCommandFileOptionDescription,
        DefaultValueFactory = _ => new FileInfo("secrets.json")
    };

    public DecryptCommand(Services.IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.DecryptCommandDescription)
    {
        Add(_userSecretsIdOption);
        Add(_inputFileOption);

        _serviceProvider = serviceProvider;

        SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
                parseResult.GetValue(_userSecretsIdOption)!,
                parseResult.GetValue(_inputFileOption)!,
                _serviceProvider.AnsiConsoleError.Value,
                _serviceProvider.SopsService.Value,
                _serviceProvider.UserSecretsService.Value,
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(string userSecretId, FileInfo inputFile, IAnsiConsole consoleError, ISopsService sopsService, IUserSecretsService userSecretsService, CancellationToken cancellationToken)
    {
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
