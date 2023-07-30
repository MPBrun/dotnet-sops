using Spectre.Console;

namespace DotnetSops.CommandLine.Services;
internal class Logger : ILogger
{
    private readonly IAnsiConsole _outConsole;
    private readonly IAnsiConsole _errorConsole;
    private readonly bool _verbose;

    public Logger(IAnsiConsole outConsole, IAnsiConsole errorConsole, bool verbose)
    {
        _outConsole = outConsole;
        _errorConsole = errorConsole;
        _verbose = verbose;
    }

    public void LogInformation(string message)
    {
        _errorConsole.MarkupLine(message);
    }

    public void LogInformationInterpolated(FormattableString message)
    {
        _errorConsole.MarkupLineInterpolated(message);
    }

    public void LogDebug(string message)
    {
        if (_verbose)
        {
            _errorConsole.MarkupLineInterpolated($"[gray]{message}[/]");
        }
    }

    public void LogWarning(string message)
    {
        _errorConsole.MarkupLineInterpolated($"[yellow]{message}[/]");
    }

    public void LogError(string message)
    {
        _errorConsole.MarkupLineInterpolated($"[red]{message}[/]");
    }

    public void LogSuccess(string message)
    {
        _outConsole.MarkupLineInterpolated($"[green]{message}[/]");
    }

    public Status Status()
    {
        return _errorConsole.Status();
    }
}
