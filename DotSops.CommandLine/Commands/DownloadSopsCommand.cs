using System.CommandLine;
using System.CommandLine.Invocation;
using DotSops.CommandLine.Binding;
using DotSops.CommandLine.Services.Sops;
using Spectre.Console;

namespace DotSops.CommandLine.Commands;
internal class DownloadSopsCommand : Command
{
    public const string CommandName = "download-sops";

    public DownloadSopsCommand()
        : base(CommandName, Properties.Resources.DownloadSopsCommandDescription)
    {
        this.SetHandler(ExecuteAsync, new InjectableBinder<InvocationContext>(), new InjectableBinder<IAnsiConsole>(), new InjectableBinder<ISopsDownloadService>());
    }

    private static async Task ExecuteAsync(InvocationContext invocationContext, IAnsiConsole console, ISopsDownloadService sopsDownloadService)
    {
        await console.Status().StartAsync(Properties.Resources.DownloadSopsLoader, async (ctx) =>
        {
            await sopsDownloadService.DownloadAsync(invocationContext.GetCancellationToken());
        });
    }
}
