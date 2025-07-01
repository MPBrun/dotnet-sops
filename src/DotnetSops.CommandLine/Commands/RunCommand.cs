using System.CommandLine;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSops.CommandLine.Commands;

internal class RunCommand : Command
{
    public const string CommandName = "run";

    private readonly IServiceProvider _serviceProvider;

    private readonly Option<FileInfo> _inputFileOption = new("--file")
    {
        Description = Properties.Resources.RunCommandFileOptionDescription,
        DefaultValueFactory = _ => new FileInfo("secrets.json"),
    };

    private readonly Argument<string[]> _runArguments = new("dotnetArguments")
    {
        Description = Properties.Resources.RunCommandArgumentsDescription,
    };

    public RunCommand(IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.RunCommandDescription)
    {
        _serviceProvider = serviceProvider;

        Add(_inputFileOption);
        Add(_runArguments);

        SetAction(
            (parseResult, cancellationToken) =>
                ExecuteAsync(
                    parseResult.GetValue(_runArguments),
                    parseResult.GetValue(_inputFileOption)!,
                    _serviceProvider.GetRequiredService<ILogger>(),
                    _serviceProvider.GetRequiredService<ISopsService>(),
                    cancellationToken
                )
        );
    }

    private static async Task<int> ExecuteAsync(
        string[]? dotnetArguments,
        FileInfo inputFile,
        ILogger logger,
        ISopsService sopsService,
        CancellationToken cancellationToken
    )
    {
        var command = "dotnet run";
        if (dotnetArguments != null)
        {
            command += " " + string.Join(" ", dotnetArguments);
        }

        logger.LogDebug($"Executing command '{command}'");

        try
        {
            await sopsService.RunCommandWithSecretsEnvironmentAsync(
                command,
                inputFile,
                cancellationToken
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
