using System.CommandLine;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;

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
                _serviceProvider.Logger.Value,
                _serviceProvider.SopsDownloadService.Value,
                cancellationToken);
        });
    }

    private static async Task<int> ExecuteAsync(ILogger logger, ISopsDownloadService sopsDownloadService, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Downloading [yellow]SOPS[/] from [link]https://github.com/getsops/sops[/]");

            await logger.Status().StartAsync(Properties.Resources.DownloadSopsLoader, async (ctx) =>
            {
                await sopsDownloadService.DownloadAsync(cancellationToken);
            });

            logger.LogSuccess("SOPS has been successfully downloaded.");
            return 0;
        }
        catch (SopsExecutionException ex)
        {
            logger.LogError(ex.Message);
            return ex.ExitCode;
        }
    }
}
