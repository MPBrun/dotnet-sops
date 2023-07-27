using System.Runtime.InteropServices;

namespace DotnetSops.CommandLine.Services.PlatformInformation;
internal interface IPlatformInformationService
{
    Architecture ProcessArchitecture { get; }

    bool IsMacOS();

    bool IsLinux();

    bool IsWindows();
}
