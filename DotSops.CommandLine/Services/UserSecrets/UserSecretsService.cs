using Microsoft.Extensions.Configuration.UserSecrets;

namespace DotSops.CommandLine.Services.UserSecrets;
internal class UserSecretsService : IUserSecretsService
{
    public FileInfo GetSecretsPathFromSecretsId(string userSecretsId)
    {
        return new FileInfo(PathHelper.GetSecretsPathFromSecretsId(userSecretsId));
    }
}
