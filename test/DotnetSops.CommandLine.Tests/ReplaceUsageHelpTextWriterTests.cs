using NSubstitute;

namespace DotnetSops.CommandLine.Tests;

public class ReplaceUsageHelpTextWriterTests
{
    [Fact]
    public void WriteBuffer_WriteToTextWriter()
    {
        // Arrange
        var textWriter = Substitute.For<TextWriter>();
        using var replaceUsageHelpTextWriter = new ReplaceUsageHelpTextWriter(textWriter);

        var buffer = new char[10];

        // Act
        replaceUsageHelpTextWriter.Write(buffer, 0, buffer.Length);

        // Assert
        textWriter.Received().Write(buffer, 0, buffer.Length);
    }

    [Fact]
    public void WriteString_WriteToTextWriterAndReplaces()
    {
        // Arrange
        var textWriter = Substitute.For<TextWriter>();
        using var replaceUsageHelpTextWriter = new ReplaceUsageHelpTextWriter(textWriter);

        // Act
        replaceUsageHelpTextWriter.Write("some text dotnet-sops after");

        // Assert
        textWriter.Received().Write("some text dotnet sops after");
    }

    [Fact]
    public void WriteChar_WriteToTextWriter()
    {
        // Arrange
        var textWriter = Substitute.For<TextWriter>();
        using var replaceUsageHelpTextWriter = new ReplaceUsageHelpTextWriter(textWriter);

        var value = 'y';

        // Act
        replaceUsageHelpTextWriter.Write(value);

        // Assert
        textWriter.Received().Write(value);
    }

    [Fact]
    public void WriteLine_WriteLineToTextWriter()
    {
        // Arrange
        var textWriter = Substitute.For<TextWriter>();
        using var replaceUsageHelpTextWriter = new ReplaceUsageHelpTextWriter(textWriter);

        // Act
        replaceUsageHelpTextWriter.WriteLine();

        // Assert
        textWriter.Received().WriteLine();
    }

    [Fact]
    public void WriteLineValue_WriteLineToTextWriter()
    {
        // Arrange
        var textWriter = Substitute.For<TextWriter>();
        using var replaceUsageHelpTextWriter = new ReplaceUsageHelpTextWriter(
            textWriter,
            "search-value"
        );

        // Act
        replaceUsageHelpTextWriter.WriteLine("some text search-value after");

        // Assert
        textWriter.Received().WriteLine("some text dotnet sops after");
    }

    [Fact]
    public void WriteLineObject_WriteLineToTextWriter()
    {
        // Arrange
        var textWriter = Substitute.For<TextWriter>();
        using var replaceUsageHelpTextWriter = new ReplaceUsageHelpTextWriter(textWriter);

        var value = new object();

        // Act
        replaceUsageHelpTextWriter.WriteLine(value);

        // Assert
        textWriter.Received().WriteLine(value);
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "VSTHRD103:Call async methods when in an async method",
        Justification = "Test case"
    )]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1849:Call async methods when in an async method",
        Justification = "Test case"
    )]
    public async Task DisposeAsync_DisposeAsyncTextWriter()
    {
        // Arrange
        var textWriter = Substitute.For<TextWriter>();
        await using (var replaceUsageHelpTextWriter = new ReplaceUsageHelpTextWriter(textWriter))
        {
            await replaceUsageHelpTextWriter.WriteLineAsync();
        }

        // Assert
        textWriter.Received().Dispose();
        await textWriter.Received().DisposeAsync();
    }

    [Fact]
    public void Dispose_DisposeTextWriter()
    {
        // Arrange
        var textWriter = Substitute.For<TextWriter>();
        using (var replaceUsageHelpTextWriter = new ReplaceUsageHelpTextWriter(textWriter))
        {
            replaceUsageHelpTextWriter.WriteLine();
        }

        // Assert
        textWriter.Received().Dispose();
    }
}
