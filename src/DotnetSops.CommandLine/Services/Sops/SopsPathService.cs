namespace DotnetSops.CommandLine.Services.Sops;
internal class SopsPathService : ISopsPathService
{
    public string GetDotnetSopsUserDirectory()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        return Path.Combine(appData, "dotnet-sops");
    }
}
