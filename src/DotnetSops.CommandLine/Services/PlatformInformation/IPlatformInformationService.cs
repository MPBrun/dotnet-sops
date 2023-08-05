using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace DotnetSops.CommandLine.Services.PlatformInformation;
internal interface IPlatformInformationService
{
    Architecture ProcessArchitecture { get; }

    [SupportedOSPlatformGuard("macOS")]
    bool IsMacOS();

    [SupportedOSPlatformGuard("linux")]
    bool IsLinux();

    [SupportedOSPlatformGuard("windows")]
    bool IsWindows();
}
