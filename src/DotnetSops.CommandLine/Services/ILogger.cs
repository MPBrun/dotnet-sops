using Spectre.Console;

namespace DotnetSops.CommandLine.Services;

internal interface ILogger
{
    public bool Verbose { get; set; }

    void LogInformation(string? message = default);

    void LogInformationInterpolated(FormattableString message);

    void LogDebug(string message);

    void LogWarning(string message);

    void LogError(string message);

    void LogSuccess(string message);

    Status Status();

    Task<string> AskAsync(string question, CancellationToken cancellationToken);

    Task<bool> ConfirmAsync(string question, CancellationToken cancellationToken);

    Task<T> SelectAsync<T>(
        string question,
        T[] choices,
        Func<T, string> converter,
        CancellationToken cancellationToken
    )
        where T : notnull;
}
