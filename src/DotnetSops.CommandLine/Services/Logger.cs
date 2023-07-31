using Spectre.Console;

namespace DotnetSops.CommandLine.Services;
internal class Logger : ILogger
{
    private readonly IAnsiConsole _outConsole;
    private readonly IAnsiConsole _errorConsole;

    public bool Verbose { get; set; }

    public Logger(IAnsiConsole outConsole, IAnsiConsole errorConsole)
    {
        _outConsole = outConsole;
        _errorConsole = errorConsole;
    }

    public void LogInformation(string? message = default)
    {
        if (message == null)
        {
            _errorConsole.WriteLine();
        }
        else
        {
            _errorConsole.MarkupLine(message);
        }
    }

    public void LogInformationInterpolated(FormattableString message)
    {
        _errorConsole.MarkupLineInterpolated(message);
    }

    public void LogDebug(string message)
    {
        if (Verbose)
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

    public async Task<string> AskAsync(string question, CancellationToken cancellationToken)
    {
        return await new Prompts.AskPrompt($"[green]?[/] {question}")
            .ShowAsync(_errorConsole, cancellationToken);
    }

    public async Task<bool> ConfirmAsync(string question, CancellationToken cancellationToken)
    {
        return await new Prompts.ConfirmationPrompt($"[green]?[/] {question}")
            .ShowAsync(_errorConsole, cancellationToken);
    }

    public async Task<T> SelectAsync<T>(string question, T[] choices, Func<T, string> converter, CancellationToken cancellationToken) where T : notnull
    {
        var selected = await new SelectionPrompt<T>()
            .Title($"[green]?[/] {question}")
            .UseConverter(converter)
            .WrapAround(true)
            .AddChoices(choices)
            .ShowAsync(_errorConsole, cancellationToken);

        _errorConsole.MarkupLineInterpolated($"[green]?[/] {question} [blue]{converter(selected)}[/]");

        return selected;
    }
}
