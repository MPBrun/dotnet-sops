using Microsoft.Extensions.Configuration.UserSecrets;

namespace DotnetSops.CommandLine.Services.UserSecrets;

internal class UserSecretsService : IUserSecretsService
{
    public FileInfo GetSecretsPathFromSecretsId(string userSecretsId)
    {
        return new FileInfo(PathHelper.GetSecretsPathFromSecretsId(userSecretsId));
    }
}
