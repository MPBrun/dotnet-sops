using DotnetSops.CommandLine.Services;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Services;
internal sealed class LoggerMock : Logger
{
    public TestConsole Out { get; }

    public TestConsole Error { get; }

    public LoggerMock(TestConsole outConsole, TestConsole erorConsole) : base(outConsole, erorConsole)
    {
        Out = outConsole;
        Error = erorConsole;

        outConsole.Profile.Capabilities.Interactive = true;
        erorConsole.Profile.Capabilities.Interactive = true;
        outConsole.Profile.Width = 1000;
        erorConsole.Profile.Width = 1000;
    }
}
