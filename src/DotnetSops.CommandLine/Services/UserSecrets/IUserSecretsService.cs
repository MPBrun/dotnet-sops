namespace DotnetSops.CommandLine.Services.UserSecrets;
internal interface IUserSecretsService
{
    FileInfo GetSecretsPathFromSecretsId(string userSecretsId);
}
