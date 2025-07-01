using System.CommandLine;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSops.CommandLine.Commands;

internal class DownloadSopsCommand : Command
{
    public const string CommandName = "download-sops";

    private readonly IServiceProvider _serviceProvider;

    public DownloadSopsCommand(IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.DownloadSopsCommandDescription)
    {
        _serviceProvider = serviceProvider;

        SetAction(
            (parseResult, cancellationToken) =>
                ExecuteAsync(
                    _serviceProvider.GetRequiredService<ILogger>(),
                    _serviceProvider.GetRequiredService<ISopsDownloadService>(),
                    cancellationToken
                )
        );
    }

    private static async Task<int> ExecuteAsync(
        ILogger logger,
        ISopsDownloadService sopsDownloadService,
        CancellationToken cancellationToken
    )
    {
        try
        {
            logger.LogInformation(Properties.Resources.DownloadSopsCommandInformation);

            await logger
                .Status()
                .StartAsync(
                    Properties.Resources.DownloadSopsLoader,
                    async (ctx) => await sopsDownloadService.DownloadAsync(cancellationToken)
                );

            logger.LogSuccess(Properties.Resources.DownloadSopsCommandSuccess);
            return 0;
        }
        catch (SopsExecutionException ex)
        {
            logger.LogError(ex.Message);
            return ex.ExitCode;
        }
    }
}
