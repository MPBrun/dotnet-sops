using System.Text;

namespace DotnetSops.CommandLine;

internal class ReplaceTextWriter : TextWriter
{
    private readonly TextWriter _inner;

    public ReplaceTextWriter(TextWriter inner)
    {
        _inner = inner;
    }

    public override Encoding Encoding => _inner.Encoding;
}
