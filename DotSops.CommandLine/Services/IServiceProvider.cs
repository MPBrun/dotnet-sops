using DotSops.CommandLine.Services.FileBom;
using DotSops.CommandLine.Services.PlatformInformation;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotSops.CommandLine.Services;
internal interface IServiceProvider
{
    Lazy<ISopsService> SopsService { get; }

    Lazy<IUserSecretsService> UserSecretsService { get; }

    Lazy<IFileBomService> FileBomService { get; }

    Lazy<ISopsDownloadService> SopsDownloadService { get; }

    Lazy<IPlatformInformationService> PlatformInformationService { get; }

    Lazy<HttpClient> HttpClient { get; }

    Lazy<IAnsiConsole> AnsiConsole { get; }
}
