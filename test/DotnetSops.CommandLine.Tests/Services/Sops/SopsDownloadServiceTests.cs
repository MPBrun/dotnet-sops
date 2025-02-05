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
    [InlineData(false, false, true, Architecture.Arm64)]
    [InlineData(false, false, true, Architecture.X64)]
    public async Task DownloadAsync_ValidOperationSystems_Success(
        bool windows,
        bool macos,
        bool linux,
        Architecture architecture
    )
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

        var service = new SopsDownloadService(
            mockPlatformInformation,
            httpClient,
            sopsPathService,
            mockLogger
        );

        // Act / Assert
        await service.DownloadAsync();
        Assert.True(true);
    }

    [Theory]
    [InlineData(false, false, false, Architecture.X64)]
    [InlineData(false, true, false, Architecture.Arm)]
    [InlineData(false, true, false, Architecture.Wasm)]
    [InlineData(false, true, false, Architecture.S390x)]
    [InlineData(false, true, false, Architecture.LoongArch64)]
    [InlineData(false, true, false, Architecture.Armv6)]
    [InlineData(false, true, false, Architecture.Ppc64le)]
#if NET9_0_OR_GREATER
    [InlineData(false, true, false, Architecture.RiscV64)]
#endif
    [InlineData(false, false, true, Architecture.X86)]
    [InlineData(false, false, true, Architecture.Arm)]
    [InlineData(false, false, true, Architecture.Wasm)]
    [InlineData(false, false, true, Architecture.S390x)]
    [InlineData(false, false, true, Architecture.LoongArch64)]
    [InlineData(false, false, true, Architecture.Armv6)]
    [InlineData(false, false, true, Architecture.Ppc64le)]
#if NET9_0_OR_GREATER
    [InlineData(false, false, true, Architecture.RiscV64)]
#endif
    public async Task DownloadAsync_InvalidOperatingSystemAndArchitecture_Throws(
        bool windows,
        bool macos,
        bool linux,
        Architecture architecture
    )
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

        var service = new SopsDownloadService(
            mockPlatformInformation,
            httpClient,
            sopsPathService,
            mockLogger
        );

        // Act / Assert
        await Assert.ThrowsAsync<NotSupportedException>(() => service.DownloadAsync());
    }

    [Fact]
    public async Task DownloadAsync_InvalidOperatingSystem_Throws()
    {
        // Arrange
        using var httpClient = new HttpClient();

        var mockPlatformInformation = Substitute.For<IPlatformInformationService>();
        var sopsPathService = new SopsPathService();
        var mockLogger = Substitute.For<ILogger>();

        var service = new SopsDownloadService(
            mockPlatformInformation,
            httpClient,
            sopsPathService,
            mockLogger
        );

        // Act / Assert
        var exception = await Assert.ThrowsAsync<NotSupportedException>(
            () => service.DownloadAsync()
        );
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
            Content = new StringContent("Invalid content"),
        };
        mockHttpClientHandler
            .ProtectedMethod<Task<HttpResponseMessage>>(
                "SendAsync",
                Arg.Any<HttpRequestMessage>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(result);
        using var httpClient = new HttpClient(mockHttpClientHandler);

        var service = new SopsDownloadService(
            mockPlatformInformation,
            httpClient,
            sopsPathService,
            mockLogger
        );

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsDownloadException>(
            () => service.DownloadAsync()
        );
        Assert.Equal(
            """
            SHA256 of SOPS executable did not match.

            Expected: bee270926fc55b5b89ed9ce87fb2569a36c74e99d63e6392090b3d0f0c2775eb
            Actual:   1cd34f3b9a3a52a0317abf2b2518511d79a18c6ac469d15617c96e18099037b3
            """,
            exception.Message
        );
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
            .ProtectedMethod<Task<HttpResponseMessage>>(
                "SendAsync",
                Arg.Any<HttpRequestMessage>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(result);
        using var httpClient = new HttpClient(mockHttpClientHandler);

        var service = new SopsDownloadService(
            mockPlatformInformation,
            httpClient,
            sopsPathService,
            mockLogger
        );

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsDownloadException>(
            () => service.DownloadAsync()
        );
        Assert.Equal(
            """
            Failed to download SOPS.

            HTTP status code: 404
            URL: https://github.com/getsops/sops/releases/download/v3.9.4/sops-v3.9.4.exe
            """,
            exception.Message
        );
    }
}
