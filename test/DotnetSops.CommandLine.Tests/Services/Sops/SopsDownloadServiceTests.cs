using System.Runtime.InteropServices;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Extensions;
using DotnetSops.CommandLine.Tests.Fixtures;
using NSubstitute;

namespace DotnetSops.CommandLine.Tests.Services.Sops;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class SopsDownloadServiceTests : IDisposable
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

    [Theory]
    [InlineData(true, false, false, Architecture.X86)]
    [InlineData(false, true, false, Architecture.X86)]
    [InlineData(false, true, false, Architecture.Arm64)]
    [InlineData(false, true, false, Architecture.X64)]
    [InlineData(false, false, true, Architecture.X86)]
    [InlineData(false, false, true, Architecture.Arm64)]
    [InlineData(false, false, true, Architecture.X64)]
    public async Task DownloadAsync_ValidOperationSystems_Success(bool windows, bool macos, bool linux, Architecture architecture)
    {
        // Arrange
        using var httpClient = new HttpClient();

        var mockPlatformInformation = Substitute.For<IPlatformInformationService>();
        mockPlatformInformation.IsWindows().Returns(windows);
        mockPlatformInformation.IsMacOS().Returns(macos);
        mockPlatformInformation.IsLinux().Returns(linux);
        mockPlatformInformation.ProcessArchitecture.Returns(architecture);

        var sopsPathService = new SopsPathService();

        var mockLogger = Substitute.For<ILogger>();

        var service = new SopsDownloadService(mockPlatformInformation, httpClient, sopsPathService, mockLogger);

        // Act / Assert
        await service.DownloadAsync();
        Assert.True(true);
    }

    [Fact]
    public async Task DownloadAsync_InvalidOperatingSystem_Throws()
    {
        // Arrange
        using var httpClient = new HttpClient();

        var mockPlatformInformation = Substitute.For<IPlatformInformationService>();
        var sopsPathService = new SopsPathService();
        var mockLogger = Substitute.For<ILogger>();

        var service = new SopsDownloadService(mockPlatformInformation, httpClient, sopsPathService, mockLogger);

        // Act / Assert
        var exception = await Assert.ThrowsAsync<NotSupportedException>(() => service.DownloadAsync());
        Assert.Equal("This operating system is not supported.", exception.Message);
    }

    [Fact]
    public async Task DownloadAsync_InvalidChecksum_Throws()
    {
        // Arrange
        var mockPlatformInformation = Substitute.For<IPlatformInformationService>();
        mockPlatformInformation.IsWindows().Returns(true);

        var sopsPathService = new SopsPathService();

        var mockLogger = Substitute.For<ILogger>();

        var mockHttpClientHandler = Substitute.For<HttpMessageHandler>();
        using var result = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent("Invalid content")
        };
        mockHttpClientHandler
            .ProtectedMethod<Task<HttpResponseMessage>>("SendAsync", Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(result);
        using var httpClient = new HttpClient(mockHttpClientHandler);

        var service = new SopsDownloadService(mockPlatformInformation, httpClient, sopsPathService, mockLogger);

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsDownloadException>(() => service.DownloadAsync());
        Assert.Equal(
            """
            SHA512 of SOPS executable did not match.

            Expected: 0B6F768D388A9316838EFC58BEBE16F0598B84F45643249996BDC6D2E237B5B01C65069809CFC153A3E97402691F4C1879D3EA4920B7DE03B38EEFD17C717C2D
            Actual:   A3E03514F3629913BCAF0B2119004854335101BDD70AEAEA3BDAB24F49C71DA8FD425BBAC89B0CE097579E8A80232116F9823D624B5A7490BC3EFF3320DBE105
            """, exception.Message);
    }

    [Fact]
    public async Task DownloadAsync_InvalidStatusCode_Throws()
    {
        // Arrange
        var mockPlatformInformation = Substitute.For<IPlatformInformationService>();
        mockPlatformInformation.IsWindows().Returns(true);

        var sopsPathService = new SopsPathService();

        var mockLogger = Substitute.For<ILogger>();

        var mockHttpClientHandler = Substitute.For<HttpMessageHandler>();
        using var result = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.NotFound,
        };
        mockHttpClientHandler
            .ProtectedMethod<Task<HttpResponseMessage>>("SendAsync", Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(result);
        using var httpClient = new HttpClient(mockHttpClientHandler);

        var service = new SopsDownloadService(mockPlatformInformation, httpClient, sopsPathService, mockLogger);

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsDownloadException>(() => service.DownloadAsync());
        Assert.Equal(
            """
            Failed to download SOPS.

            HTTP status code: 404
            URL: https://github.com/getsops/sops/releases/download/v3.7.3/sops-v3.7.3.exe
            """, exception.Message);
    }
}
