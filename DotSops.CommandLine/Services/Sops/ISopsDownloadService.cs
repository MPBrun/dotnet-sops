namespace DotSops.CommandLine.Services.Sops;
internal interface ISopsDownloadService
{
    Task DownloadAsync(CancellationToken cancellationToken = default);
}
