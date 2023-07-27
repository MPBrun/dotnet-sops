using System.Runtime.InteropServices;
using DotnetSops.CommandLine.Services.PlatformInformation;

namespace DotnetSops.CommandLine.Tests.Services.PlatformInformation;
public class PlatformInformationTests
{
    [Fact]
    public void ProcessArchitecture_Returns()
    {
        // Arrange
        var service = new PlatformInformationService();

        // Act
        var arc = service.ProcessArchitecture;

        // Assert
        Assert.Equal(RuntimeInformation.ProcessArchitecture, arc);
    }

    [Fact]
    public void IsLinux_Returns()
    {
        // Arrange
        var service = new PlatformInformationService();

        // Act
        var isLinux = service.IsLinux();

        // Assert
        Assert.Equal(OperatingSystem.IsLinux(), isLinux);
    }

    [Fact]
    public void IsMacOS_Returns()
    {
        // Arrange
        var service = new PlatformInformationService();

        // Act
        var isMacOS = service.IsMacOS();

        // Assert
        Assert.Equal(OperatingSystem.IsMacOS(), isMacOS);
    }

    [Fact]
    public void IsWindows_Returns()
    {
        // Arrange
        var service = new PlatformInformationService();

        // Act
        var isWindows = service.IsWindows();

        // Assert
        Assert.Equal(OperatingSystem.IsWindows(), isWindows);
    }
}
