using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.Sops;
using Moq;

namespace DotnetSops.CommandLine.Tests.Fixtures;
public class SopsFixture : IAsyncLifetime
{
    private string _sopsDirectory = "";

    public Task DisposeAsync()
    {
        Directory.Delete(_sopsDirectory, true);
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var fixtureDir = Directory.CreateDirectory("sops-executables");

        _sopsDirectory = fixtureDir.FullName;
        Directory.SetCurrentDirectory(_sopsDirectory);

        using var httpClient = new HttpClient();
        var platformInformation = new PlatformInformationService();
        var mockLogger = new Mock<ILogger>();

        var service = new SopsDownloadService(platformInformation, httpClient, mockLogger.Object);
        await service.DownloadAsync();

        Directory.SetCurrentDirectory(currentDir);
    }

    public void CopySopsToDirectory(string directory)
    {
        foreach (var sopsFilePath in Directory.EnumerateFiles(_sopsDirectory))
        {
            File.Copy(sopsFilePath, Path.Combine(directory, Path.GetFileName(sopsFilePath)));
        }
    }
}
