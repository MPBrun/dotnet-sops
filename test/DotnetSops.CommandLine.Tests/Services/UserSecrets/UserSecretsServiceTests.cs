using DotnetSops.CommandLine.Services.UserSecrets;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace DotnetSops.CommandLine.Tests.Services.UserSecrets;

public class UserSecretsServiceTests
{
    [Fact]
    public void GetSecretsPathFromSecretsId_ValidUserSecretId_Returns()
    {
        // Arrange
        var service = new UserSecretsService();
        var userSecretId = Guid.NewGuid().ToString();

        // Act
        var path = service.GetSecretsPathFromSecretsId(userSecretId);

        // Assert
        Assert.Equal(PathHelper.GetSecretsPathFromSecretsId(userSecretId), path.FullName);
    }
}
