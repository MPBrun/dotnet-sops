using DotnetSops.CommandLine.Services.Sops;

namespace DotnetSops.CommandLine.Tests.Services.Sops;

public class SopsPathServiceTests
{
    [Fact]
    public void GetDotnetSopsUserDirectory_AppData_ReturnDirectoryInApplicationData()
    {
        // Arrange
        var sopsPathService = new SopsPathService();

        // Act
        var path = sopsPathService.GetDotnetSopsUserDirectory();

        // Assert
        Assert.Equal(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "dotnet-sops"), path);
    }
}
