using System.Text;

namespace DotnetSops.CommandLine;

/// <summary>
/// TextWriter that replaces the help usage 'dotnet-sops' to the correct usage 'dotnet sops'
/// </summary>
internal class ReplaceUsageHelpTextWriter : TextWriter
{
    private const string Replacement = "dotnet sops";

    private readonly TextWriter _textWriter;
    private readonly string _searchValue;

    public override Encoding Encoding => _textWriter.Encoding;

    public ReplaceUsageHelpTextWriter(TextWriter textWriter, string searchValue = "dotnet-sops")
    {
        _textWriter = textWriter;
        _searchValue = searchValue;
    }

    public override void Write(char[] buffer, int index, int count)
    {
        _textWriter.Write(buffer, index, count);
    }

    public override void Write(string? value)
    {
        _textWriter.Write(value?.Replace(_searchValue, Replacement, StringComparison.InvariantCulture));
    }

    public override void Write(char value)
    {
        _textWriter.Write(value);
    }

    public override void WriteLine()
    {
        _textWriter.WriteLine();
    }

    public override void WriteLine(string? value)
    {
        _textWriter.WriteLine(value?.Replace(_searchValue, Replacement, StringComparison.InvariantCulture));
    }

    public override void WriteLine(object? value)
    {
        _textWriter.WriteLine(value);
    }

    protected override void Dispose(bool disposing)
    {
        _textWriter.Dispose();
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        await _textWriter.DisposeAsync();
        await base.DisposeAsync();
    }
}
