using System.CommandLine;
using DotSops.CommandLine.Services.Sops;
using Spectre.Console;

namespace DotSops.CommandLine.Commands;
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
              _serviceProvider.AnsiConsole.Value,
              _serviceProvider.SopsDownloadService.Value,
              cancellationToken);
        });
    }

    private static async Task ExecuteAsync(IAnsiConsole console, ISopsDownloadService sopsDownloadService, CancellationToken cancellationToken)
    {
        await console.Status().StartAsync(Properties.Resources.DownloadSopsLoader, async (ctx) =>
        {
            await sopsDownloadService.DownloadAsync(cancellationToken);
        });
    }
}
