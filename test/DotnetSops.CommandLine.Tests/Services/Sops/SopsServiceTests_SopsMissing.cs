using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Fixtures;
using Moq;

namespace DotnetSops.CommandLine.Tests.Services.Sops;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class SopsServiceTests_SopsMissing : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();

    protected virtual void Dispose(bool disposing)
    {
        _uniqueCurrentDirectoryFixture.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task EncryptAsync_NoSops_ThrowsSopsMissingException()
    {
        // Arrange
        var logger = new Mock<ILogger>();
        var sopsService = new SopsService(logger.Object);
        var fileName = new FileInfo("secrets.json");
        var encrypedFile = new FileInfo("encrypted.json");

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsMissingException>(() => sopsService.EncryptAsync(fileName, encrypedFile));
        Assert.Equal("SOPS executable could not be found on the PATH.", exception.Message);
    }
}
