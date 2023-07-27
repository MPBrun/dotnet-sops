namespace DotnetSops.CommandLine.Services.Sops;
internal interface ISopsDownloadService
{
    /// <summary>
    /// Download sops from github
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="SopsDownloadException"></exception>
    Task DownloadAsync(CancellationToken cancellationToken = default);
}
