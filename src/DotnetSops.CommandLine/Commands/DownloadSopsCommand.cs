using System.CommandLine;
using DotnetSops.CommandLine.Services.Sops;
using Spectre.Console;

namespace DotnetSops.CommandLine.Commands;
internal class DownloadSopsCommand : CliCommand
{
    public const string CommandName = "download-sops";

    private readonly Services.IServiceProvider _serviceProvider;

    public DownloadSopsCommand(Services.IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.DownloadSopsCommandDescription)
    {
        _serviceProvider = serviceProvider;

        SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
                _serviceProvider.AnsiConsoleError.Value,
                _serviceProvider.SopsDownloadService.Value,
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(IAnsiConsole consoleError, ISopsDownloadService sopsDownloadService, CancellationToken cancellationToken)
    {
        try
        {
            consoleError.MarkupLine("Downloading [green]SOPS[/] from [link]https://github.com/getsops/sops[/]");

            await consoleError.Status().StartAsync(Properties.Resources.DownloadSopsLoader, async (ctx) =>
            {
                await sopsDownloadService.DownloadAsync(cancellationToken);
            });

            consoleError.MarkupLine("[green]SOPS has been successfully downloaded.[/]");
            return 0;
        }
        catch (SopsExecutionException ex)
        {
            consoleError.WriteLine(ex.Message);
            return ex.ExitCode;
        }
    }
}
