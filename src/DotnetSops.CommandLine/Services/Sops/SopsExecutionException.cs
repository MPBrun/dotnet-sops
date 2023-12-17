namespace DotnetSops.CommandLine.Services.Sops;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1064:Exceptions should be public",
    Justification = "Used internal only."
)]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Critical Code Smell",
    "S3871:Exception types should be \"public\"",
    Justification = "Used internal only."
)]
internal class SopsExecutionException : Exception
{
    public required int ExitCode { get; init; }

    public SopsExecutionException() { }

    public SopsExecutionException(string message)
        : base(message) { }

    public SopsExecutionException(string message, Exception innerException)
        : base(message, innerException) { }
}
