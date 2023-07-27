using System.Runtime.InteropServices;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.Sops;
using Moq;
using Moq.Protected;

namespace DotnetSops.CommandLine.Tests.Services.Sops;

public class SopsDownloadServiceTests
{
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

        var mockPlatformInformation = new Mock<IPlatformInformationService>();
        mockPlatformInformation.Setup(p => p.IsWindows()).Returns(windows);
        mockPlatformInformation.Setup(p => p.IsMacOS()).Returns(macos);
        mockPlatformInformation.Setup(p => p.IsLinux()).Returns(linux);
        mockPlatformInformation.Setup(p => p.ProcessArchitecture).Returns(architecture);

        var service = new SopsDownloadService(mockPlatformInformation.Object, httpClient);

        // Act / Assert
        await service.DownloadAsync();
        Assert.True(true);
    }

    [Fact]
    public async Task DownloadAsync_InvalidOperatingSystem_Throws()
    {
        // Arrange
        using var httpClient = new HttpClient();

        var mockPlatformInformation = new Mock<IPlatformInformationService>();

        var service = new SopsDownloadService(mockPlatformInformation.Object, httpClient);

        // Act / Assert
        var exception = await Assert.ThrowsAsync<NotSupportedException>(() => service.DownloadAsync());
        Assert.Equal("[red]Operating system is not supported.[/]", exception.Message);
    }

    [Fact]
    public async Task DownloadAsync_InvalidChecksum_Throws()
    {
        // Arrange
        var mockPlatformInformation = new Mock<IPlatformInformationService>();
        mockPlatformInformation.Setup(p => p.IsWindows()).Returns(true);

        var mockHttpClientHandler = new Mock<HttpMessageHandler>();
        using var result = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent("Invalid content")
        };
        mockHttpClientHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(result);
        using var httpClient = new HttpClient(mockHttpClientHandler.Object);

        var service = new SopsDownloadService(mockPlatformInformation.Object, httpClient);

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsDownloadException>(() => service.DownloadAsync());
        Assert.Equal(
            """
            [red]SHA512 of SOPS executable did not match.[/]

            [yellow]Expected: 0B6F768D388A9316838EFC58BEBE16F0598B84F45643249996BDC6D2E237B5B01C65069809CFC153A3E97402691F4C1879D3EA4920B7DE03B38EEFD17C717C2D[/]
            [yellow]Actual:   A3E03514F3629913BCAF0B2119004854335101BDD70AEAEA3BDAB24F49C71DA8FD425BBAC89B0CE097579E8A80232116F9823D624B5A7490BC3EFF3320DBE105[/]
            """, exception.Message);
    }

    [Fact]
    public async Task DownloadAsync_InvalidStatusCode_Throws()
    {
        // Arrange
        var mockPlatformInformation = new Mock<IPlatformInformationService>();
        mockPlatformInformation.Setup(p => p.IsWindows()).Returns(true);

        var mockHttpClientHandler = new Mock<HttpMessageHandler>();
        using var result = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.NotFound,
        };
        mockHttpClientHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(result);
        using var httpClient = new HttpClient(mockHttpClientHandler.Object);

        var service = new SopsDownloadService(mockPlatformInformation.Object, httpClient);

        // Act / Assert
        var exception = await Assert.ThrowsAsync<SopsDownloadException>(() => service.DownloadAsync());
        Assert.Equal(
            """
            [red]Failed to download SOPS.[/]

            [yellow]HTTP status code: 404[/]
            [yellow]URL: https://github.com/getsops/sops/releases/download/v3.7.3/sops-v3.7.3.exe[/]
            """, exception.Message);
    }
}
