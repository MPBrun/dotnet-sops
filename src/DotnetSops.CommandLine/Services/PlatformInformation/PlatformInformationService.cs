using System.Runtime.InteropServices;

namespace DotnetSops.CommandLine.Services.PlatformInformation;

internal class PlatformInformationService : IPlatformInformationService
{
    public Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;

    public bool IsLinux()
    {
        return OperatingSystem.IsLinux();
    }

    public bool IsMacOS()
    {
        return OperatingSystem.IsMacOS();
    }

    public bool IsWindows()
    {
        return OperatingSystem.IsWindows();
    }
}
