namespace DotnetSops.CommandLine.Services.ProjectInfo;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "Used internal only.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3871:Exception types should be \"public\"", Justification = "Used internal only.")]
internal class ProjectInfoSearchException : Exception
{
    public string? Suggestion { get; init; }

    public ProjectInfoSearchException()
    {
    }

    public ProjectInfoSearchException(string message) : base(message)
    {
    }

    public ProjectInfoSearchException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
