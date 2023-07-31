using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using Moq;

namespace DotnetSops.CommandLine.Tests.Services.Sops;
public class SopsServiceTests_SopsMissing
{
    [Fact]
    public async Task EncryptAsync_Valid_Encrypts()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var logger = new Mock<ILogger>();
        var sopsService = new SopsService(dir.FullName, logger.Object);
        var fileName = new FileInfo(Path.Combine(dir.FullName, "secrets.json"));
        var encrypedFile = new FileInfo(Path.Combine(dir.FullName, "encrypted.json"));

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsMissingException>(() => sopsService.EncryptAsync(fileName, encrypedFile));
        Assert.Equal("SOPS executable could not be found on the PATH.", exception.Message);
    }
}
