using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotnetSops.CommandLine.Tests.Services;

internal sealed class MockServiceProvider : CommandLine.Services.IServiceProvider
{
    public Lazy<ISopsService> SopsService { get; set; } = default!;

    public Lazy<IUserSecretsService> UserSecretsService { get; set; } = default!;

    public Lazy<IFileBomService> FileBomService { get; set; } = default!;

    public Lazy<ISopsDownloadService> SopsDownloadService { get; set; } = default!;

    public Lazy<IPlatformInformationService> PlatformInformationService { get; set; } = default!;

    public Lazy<HttpClient> HttpClient { get; set; } = default!;

    public Lazy<IAnsiConsole> AnsiConsoleError { get; set; } = default!;

    public Lazy<IAnsiConsole> AnsiConsoleOut { get; set; } = default!;

    public bool Verbose { get; set; }
}
