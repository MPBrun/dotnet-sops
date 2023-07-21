using DotSops.CommandLine.Services.FileBom;
using DotSops.CommandLine.Services.PlatformInformation;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

namespace DotSops.CommandLine.Tests.Services;

internal sealed class MockServiceProvider : CommandLine.Services.IServiceProvider
{
    public Lazy<ISopsService> SopsService { get; set; } = default!;

    public Lazy<IUserSecretsService> UserSecretsService { get; set; } = default!;

    public Lazy<IFileBomService> FileBomService { get; set; } = default!;

    public Lazy<ISopsDownloadService> SopsDownloadService { get; set; } = default!;

    public Lazy<IPlatformInformationService> PlatformInformationService { get; set; } = default!;

    public Lazy<HttpClient> HttpClient { get; set; } = default!;

    public Lazy<IAnsiConsole> AnsiConsole { get; set; } = default!;
}
