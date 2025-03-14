using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.Sops;
using NSubstitute;

namespace DotnetSops.CommandLine.Tests.Fixtures;

public class SopsFixture : IAsyncLifetime
{
    internal ISopsPathService SopsPathService { get; private set; } = default!;

    public ValueTask DisposeAsync()
    {
        Directory.Delete(SopsPathService.GetDotnetSopsUserDirectory(), true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public async ValueTask InitializeAsync()
    {
        var fixtureDir = Path.Join(Directory.GetCurrentDirectory(), "sops-executables");

        using var httpClient = new HttpClient();
        var platformInformation = new PlatformInformationService();
        var logger = Substitute.For<ILogger>();
        SopsPathService = Substitute.For<ISopsPathService>();
        SopsPathService.GetDotnetSopsUserDirectory().Returns(fixtureDir);

        var service = new SopsDownloadService(
            platformInformation,
            httpClient,
            SopsPathService,
            logger
        );
        await service.DownloadAsync();
    }
}
