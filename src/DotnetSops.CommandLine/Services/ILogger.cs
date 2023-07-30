using Spectre.Console;

namespace DotnetSops.CommandLine.Services;
internal interface ILogger
{
    void LogInformation(string message);

    void LogInformationInterpolated(FormattableString message);

    void LogDebug(string message);

    void LogWarning(string message);

    void LogError(string message);

    void LogSuccess(string message);

    Status Status();
}
