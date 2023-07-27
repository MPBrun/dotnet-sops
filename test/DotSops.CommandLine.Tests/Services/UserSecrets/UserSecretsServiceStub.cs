using DotSops.CommandLine.Services.UserSecrets;

namespace DotSops.CommandLine.Tests.Services.UserSecrets;
internal sealed class UserSecretsServiceStub : IUserSecretsService
{
    private readonly string _rootDir;

    public UserSecretsServiceStub(string rootDir)
    {
        _rootDir = rootDir;
    }

    public FileInfo GetSecretsPathFromSecretsId(string userSecretsId)
    {
        Directory.CreateDirectory($"{_rootDir}/{userSecretsId}");
        return new FileInfo($"{_rootDir}/{userSecretsId}/secrets.json");
    }
}
