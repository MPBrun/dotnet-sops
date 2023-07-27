using System.CommandLine;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Microsoft.Build.Construction;
using Microsoft.Build.Exceptions;
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
                _serviceProvider.SopsService.Value,
                _serviceProvider.UserSecretsService.Value,
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(FileInfo? projectFile, string? userSecretId, FileInfo inputFile, IAnsiConsole consoleError, ISopsService sopsService, IUserSecretsService userSecretsService, CancellationToken cancellationToken)
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
                var project = ProjectRootElement.Open(projectFile.FullName);
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
