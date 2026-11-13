using System.Text;

namespace DotnetSops.CommandLine;

/// <summary>
/// TextWriter that replaces the help usage 'dotnet-sops' to the correct usage 'dotnet sops'
/// </summary>
internal class ReplaceUsageHelpTextWriter : TextWriter
{
    private const string Replacement = "dotnet sops";

    private readonly TextWriter _textWriter;
    private readonly string[] _searchValues;

    public override Encoding Encoding => _textWriter.Encoding;

    public ReplaceUsageHelpTextWriter(TextWriter textWriter)
        : this(textWriter, ["dotnet-sops", "DotnetSops.CommandLine"]) { }

    public ReplaceUsageHelpTextWriter(TextWriter textWriter, string[] searchValues)
    {
        _textWriter = textWriter;
        _searchValues = searchValues;
    }

    public override void Write(char[] buffer, int index, int count)
    {
        _textWriter.Write(buffer, index, count);
    }

    public override void Write(string? value)
    {
        var valueToWrite = ReplaceSearchValuesWithReplacement(value);
        _textWriter.Write(valueToWrite);
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
        var valueToWrite = ReplaceSearchValuesWithReplacement(value);
        _textWriter.WriteLine(valueToWrite);
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

    private string? ReplaceSearchValuesWithReplacement(string? value)
    {
        var valueToWrite = value;
        foreach (var searchValue in _searchValues)
        {
            valueToWrite = valueToWrite?.Replace(
                searchValue,
                Replacement,
                StringComparison.InvariantCulture
            );
        }

        return valueToWrite;
    }
}
