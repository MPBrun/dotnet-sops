using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Fixtures;
using NSubstitute;

namespace DotnetSops.CommandLine.Tests.Services.Sops;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class SopsServiceTests_SopsMissing : IDisposable
{
    private const string PathEnvironmentVariableName = "PATH";
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();
    private readonly string? _originalPathEnviromentVariableValue;

    public SopsServiceTests_SopsMissing()
    {
        _originalPathEnviromentVariableValue = Environment.GetEnvironmentVariable(
            PathEnvironmentVariableName
        );
    }

    protected virtual void Dispose(bool disposing)
    {
        _uniqueCurrentDirectoryFixture.Dispose();
        Environment.SetEnvironmentVariable(
            PathEnvironmentVariableName,
            _originalPathEnviromentVariableValue
        );
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
        Environment.SetEnvironmentVariable(PathEnvironmentVariableName, null);

        var logger = Substitute.For<ILogger>();
        var sopsPathService = Substitute.For<ISopsPathService>();
        var sopsService = new SopsService(logger, sopsPathService);
        var fileName = new FileInfo("secrets.json");
        var encrypedFile = new FileInfo("encrypted.json");

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsMissingException>(
            () =>
                sopsService.EncryptAsync(
                    fileName,
                    encrypedFile,
                    TestContext.Current.CancellationToken
                )
        );
        Assert.Equal("SOPS executable could not be found on the PATH.", exception.Message);
    }
}
