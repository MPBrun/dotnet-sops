namespace DotnetSops.CommandLine.Services.Sops;
internal class SopsPathService : ISopsPathService
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Conflicts with S3358")]
    public string GetDotnetSopsUserDirectory()
    {
        var appData = Environment.GetEnvironmentVariable("APPDATA");
        var root = appData                                          // On Windows it goes to %APPDATA%\dotnet-sops\
                   ?? Environment.GetEnvironmentVariable("HOME")    // On Mac/Linux it goes to ~/.dotnet-sops/
                   ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                   ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (string.IsNullOrEmpty(root))
        {
            return Directory.GetCurrentDirectory();
        }
        else
        {
            return !string.IsNullOrEmpty(appData) ? Path.Combine(root, "dotnet-sops") : Path.Combine(root, ".dotnet-sops");
        }
    }
}
