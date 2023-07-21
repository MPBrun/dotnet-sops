using DotSops.CommandLine.Services.FileBom;
using DotSops.CommandLine.Services.PlatformInformation;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotSops.CommandLine.Services;
internal class ServiceProvider : IServiceProvider
{
    public Lazy<ISopsService> SopsService => new(() => new SopsService());

    public Lazy<IUserSecretsService> UserSecretsService => new(() => new UserSecretsService());

    public Lazy<IFileBomService> FileBomService => new(() => new FileBomService());

    public Lazy<ISopsDownloadService> SopsDownloadService => new(() => new SopsDownloadService(PlatformInformationService.Value, HttpClient.Value));

    public Lazy<IPlatformInformationService> PlatformInformationService => new(() => new PlatformInformationService());

    public Lazy<HttpClient> HttpClient => new(() => new HttpClient());

    public Lazy<IAnsiConsole> AnsiConsole => new(() =>
    {
        return Spectre.Console.AnsiConsole.Create(new AnsiConsoleSettings()
        {
            Ansi = AnsiSupport.Detect,
            ColorSystem = ColorSystemSupport.Detect,
            Out = new AnsiConsoleOutput(Console.Error),
        });
    });
}
