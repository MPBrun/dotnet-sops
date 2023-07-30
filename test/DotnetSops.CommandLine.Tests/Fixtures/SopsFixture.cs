using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.Sops;

namespace DotnetSops.CommandLine.Tests.Fixtures;
public class SopsFixture : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        if (File.Exists("sops.exe") || File.Exists("sops"))
        {
            return;
        }
        using var httpClient = new HttpClient();
        var platformInformation = new PlatformInformationService();

        var service = new SopsDownloadService(platformInformation, httpClient);
        await service.DownloadAsync();
    }
}
