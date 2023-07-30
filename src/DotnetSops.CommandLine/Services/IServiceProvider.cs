using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotnetSops.CommandLine.Services;
internal interface IServiceProvider
{
    Lazy<ISopsService> SopsService { get; }

    Lazy<IUserSecretsService> UserSecretsService { get; }

    Lazy<IFileBomService> FileBomService { get; }

    Lazy<ISopsDownloadService> SopsDownloadService { get; }

    Lazy<IPlatformInformationService> PlatformInformationService { get; }

    Lazy<IProjectInfoService> ProjectInfoService { get; }

    Lazy<HttpClient> HttpClient { get; }

    Lazy<IAnsiConsole> AnsiConsoleError { get; }

    Lazy<IAnsiConsole> AnsiConsoleOut { get; }

    Lazy<ILogger> Logger { get; }

    bool Verbose { get; set; }
}
