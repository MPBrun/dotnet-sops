namespace DotSops.CommandLine;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "Used internal only.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3871:Exception types should be \"public\"", Justification = "Used internal only.")]
internal class DotSopsException : Exception
{
    public DotSopsException()
    {
    }

    public DotSopsException(string message) : base(message)
    {
    }

    public DotSopsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
