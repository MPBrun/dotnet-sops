using System.CommandLine;
using DotSops.CommandLine.Services.FileBom;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;
using Microsoft.Build.Construction;
using Microsoft.Build.Exceptions;
using Spectre.Console;

namespace DotSops.CommandLine.Commands;

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
                _serviceProvider.SopsService.Value,
                _serviceProvider.UserSecretsService.Value,
                _serviceProvider.FileBomService.Value,
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(FileInfo? projectFile, string? userSecretId, FileInfo outputFile, IAnsiConsole consoleError, ISopsService sopsService, IUserSecretsService userSecretsService, IFileBomService fileBomService, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userSecretId))
        {
            if (projectFile == null)
            {
                var projectFiles = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.*proj", SearchOption.TopDirectoryOnly)
                    .Where(f => !".xproj".Equals(Path.GetExtension(f), StringComparison.OrdinalIgnoreCase))
                    .Select(file => new FileInfo(file))
                    .ToList();
                if (projectFiles.Count > 1)
                {
                    consoleError.MarkupLineInterpolated($"""
                        [red]Multiple MSBuild project files found in '{Directory.GetCurrentDirectory()}'.
                        Specify which to use with the --project option[/]
                        """);
                    return 1;
                }
                else if (projectFiles.Count == 0)
                {
                    consoleError.MarkupLineInterpolated($"""
                        [red]Could not find a MSBuild project file in '{Directory.GetCurrentDirectory()}'.
                        Specify which project to use with the --project option or use the '--id' option.[/]
                        """);
                    return 1;
                }
                else
                {
                    projectFile = projectFiles[0];
                }
            }

            if (!projectFile.Exists)
            {
                consoleError.MarkupLine(LocalizationResources.FileDoesNotExist(projectFile.FullName));
                return 1;
            }
            try
            {
                var project = ProjectRootElement.Open(projectFile.FullName); // TODO: Replace with XDocument. https://github.com/dotnet/aspnetcore/blob/f9828517e57769959d43c89d06ad7f319880258c/src/Tools/Shared/SecretsHelpers/UserSecretsCreator.cs#L31
                userSecretId = project.Properties.FirstOrDefault(p => p.Name == "UserSecretsId")?.Value;

                if (string.IsNullOrWhiteSpace(userSecretId))
                {
                    consoleError.MarkupLineInterpolated($"""
                        [red]Could not find the global property 'UserSecretsId' in MSBuild project '{projectFile.FullName}'
                        Ensure this property is set in the project or use the '--id' command line option.[/]
                        """);
                    return 1;
                }
            }
            catch (InvalidProjectFileException)
            {
                consoleError.MarkupLineInterpolated($"""
                        [red]Could not load the MSBuild project '{projectFile.FullName}'.[/]
                        """);
                return 1;
            }
        }

        var inputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);
        if (!inputFile.Exists)
        {
            // TODO: Just return code 0: "No secrets configured for this application."
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
