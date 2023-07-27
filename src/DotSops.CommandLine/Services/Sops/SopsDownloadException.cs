namespace DotSops.CommandLine.Services.Sops;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "Used internal only.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3871:Exception types should be \"public\"", Justification = "Used internal only.")]
internal class SopsDownloadException : Exception
{
    public SopsDownloadException()
    {
    }

    public SopsDownloadException(string message) : base(message)
    {
    }

    public SopsDownloadException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
