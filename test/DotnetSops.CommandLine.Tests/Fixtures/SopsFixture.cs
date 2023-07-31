using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.Sops;
using Moq;

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
        var mockLogger = new Mock<ILogger>();

        var service = new SopsDownloadService(platformInformation, httpClient, mockLogger.Object);
        await service.DownloadAsync();
    }
}
