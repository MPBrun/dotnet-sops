using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotnetSops.CommandLine.Services;
internal class ServiceProvider : IServiceProvider
{
    public Lazy<ISopsService> SopsService => new(() => new SopsService());

    public Lazy<IUserSecretsService> UserSecretsService => new(() => new UserSecretsService());

    public Lazy<IFileBomService> FileBomService => new(() => new FileBomService());

    public Lazy<ISopsDownloadService> SopsDownloadService => new(() => new SopsDownloadService(PlatformInformationService.Value, HttpClient.Value));

    public Lazy<IPlatformInformationService> PlatformInformationService => new(() => new PlatformInformationService());

    public Lazy<IProjectInfoService> ProjectInfoService => new(() => new ProjectInfoService());

    public Lazy<HttpClient> HttpClient => new(() => new HttpClient());

    public Lazy<IAnsiConsole> AnsiConsoleError => new(() =>
    {
        return AnsiConsole.Create(new AnsiConsoleSettings()
        {
            Ansi = AnsiSupport.Detect,
            ColorSystem = ColorSystemSupport.Detect,
            Out = new AnsiConsoleOutput(Console.Error),
        });
    });

    public Lazy<IAnsiConsole> AnsiConsoleOut => new(() =>
    {
        return AnsiConsole.Create(new AnsiConsoleSettings()
        {
            Ansi = AnsiSupport.Detect,
            ColorSystem = ColorSystemSupport.Detect,
            Out = new AnsiConsoleOutput(Console.Out),
        });
    });

    public Lazy<ILogger> Logger => new(() => new Logger(AnsiConsoleOut.Value, AnsiConsoleError.Value, Verbose));

    public bool Verbose { get; set; }
}
