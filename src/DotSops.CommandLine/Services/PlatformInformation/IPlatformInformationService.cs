using System.Runtime.InteropServices;

namespace DotSops.CommandLine.Services.PlatformInformation;
internal interface IPlatformInformationService
{
    Architecture ProcessArchitecture { get; }

    bool IsMacOS();

    bool IsLinux();

    bool IsWindows();
}
