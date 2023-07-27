namespace DotnetSops.CommandLine.Services.Sops;
internal interface ISopsDownloadService
{
    Task DownloadAsync(CancellationToken cancellationToken = default);
}
