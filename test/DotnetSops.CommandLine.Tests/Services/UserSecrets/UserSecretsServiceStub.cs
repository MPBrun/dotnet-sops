using DotnetSops.CommandLine.Services.UserSecrets;

namespace DotnetSops.CommandLine.Tests.Services.UserSecrets;
internal sealed class UserSecretsServiceStub : IUserSecretsService
{
    private readonly string _rootDir;

    public UserSecretsServiceStub(string rootDir)
    {
        _rootDir = rootDir;
    }

    public FileInfo GetSecretsPathFromSecretsId(string userSecretsId)
    {
        Directory.CreateDirectory(Path.Join(_rootDir, userSecretsId));
        return new FileInfo(Path.Join(_rootDir, userSecretsId, "secrets.json"));
    }
}
