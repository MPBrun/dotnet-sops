using DotnetSops.CommandLine.Services;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Services;

public class LoggerTests : IDisposable
{
    private readonly TestConsole _out;
    private readonly TestConsole _error;

    protected virtual void Dispose(bool disposing)
    {
        _out.Dispose();
        _error.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public LoggerTests()
    {
        _out = new TestConsole();
        _out.Profile.Capabilities.Interactive = true;
        _out.Profile.Width = 1000;

        _error = new TestConsole();
        _error.Profile.Capabilities.Interactive = true;
        _error.Profile.Width = 1000;
    }

    [Fact]
    public void LogInformation_Null_WriteNewLine()
    {
        // Arrange
        var logger = new Logger(_out, _error);

        // Act
        logger.LogInformation();

        // Assert
        Assert.Equal("\n", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public void LogInformation_Value_WriteValue()
    {
        // Arrange
        var logger = new Logger(_out, _error);

        // Act
        logger.LogInformation("some text");

        // Assert
        Assert.Equal("some text\n", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public void LogInformationInterpolated_Value_WriteValue()
    {
        // Arrange
        var logger = new Logger(_out, _error);

        // Act
        logger.LogInformationInterpolated($"{"foo"} {"bar"}");

        // Assert
        Assert.Equal("foo bar\n", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public void LogDebug_VerboseDisabled_WriteNothing()
    {
        // Arrange
        var logger = new Logger(_out, _error);

        // Act
        logger.LogDebug("some text");

        // Assert
        Assert.Equal("", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public void LogDebug_VerboseEnabled_WriteNothing()
    {
        // Arrange
        var logger = new Logger(_out, _error) { Verbose = true };

        // Act
        logger.LogDebug("some text");

        // Assert
        Assert.Equal("some text\n", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public void LogWarning_WriteValue()
    {
        // Arrange
        var logger = new Logger(_out, _error) { Verbose = true };

        // Act
        logger.LogWarning("some text");

        // Assert
        Assert.Equal("some text\n", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public void LogError_WriteValue()
    {
        // Arrange
        var logger = new Logger(_out, _error) { Verbose = true };

        // Act
        logger.LogError("some text");

        // Assert
        Assert.Equal("some text\n", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public void LogSuccess_WriteValueToOut()
    {
        // Arrange
        var logger = new Logger(_out, _error) { Verbose = true };

        // Act
        logger.LogSuccess("some text");

        // Assert
        Assert.Equal("", _error.Output);
        Assert.Equal("some text\n", _out.Output);
    }

    [Fact]
    public async Task AskAsync_Prompts()
    {
        // Arrange
        var logger = new Logger(_out, _error) { Verbose = true };

        _error.Input.PushTextWithEnter("42");

        // Act
        var result = await logger.AskAsync("What are the meaning of life?", CancellationToken.None);

        // Assert
        Assert.Equal("42", result);
        Assert.Equal("? What are the meaning of life? 42\n", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public async Task ConfirmAsync_Prompts()
    {
        // Arrange
        var logger = new Logger(_out, _error) { Verbose = true };

        _error.Input.PushTextWithEnter("y");

        // Act
        var result = await logger.ConfirmAsync("Is this great?", CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal("? Is this great? [y/n] (n): y\n", _error.Output);
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public async Task SelectAsync_Prompts()
    {
        // Arrange
        var logger = new Logger(_out, _error) { Verbose = true };

        _error.Input.PushKey(ConsoleKey.DownArrow);
        _error.Input.PushKey(ConsoleKey.Enter);

        var choices = new[] { "Red", "Green", "Blue" };

        // Act
        var result = await logger.SelectAsync(
            "What is your favorite color?",
            choices,
            (choice) => choice,
            CancellationToken.None
        );

        // Assert
        Assert.Equal("Green", result);
        Assert.Equal(
            """
            ? What is your favorite color?
                                          
            > Red                         
              Green                       
              Blue                        ? What is your favorite color?
                                          
              Red                         
            > Green                       
              Blue                        ? What is your favorite color? Green

            """,
            _error.Output,
            ignoreLineEndingDifferences: true
        );
        Assert.Equal("", _out.Output);
    }

    [Fact]
    public async Task SelectAsync_PromptWraps()
    {
        // Arrange
        var logger = new Logger(_out, _error) { Verbose = true };

        _error.Input.PushKey(ConsoleKey.DownArrow);
        _error.Input.PushKey(ConsoleKey.DownArrow);
        _error.Input.PushKey(ConsoleKey.DownArrow);
        _error.Input.PushKey(ConsoleKey.DownArrow);
        _error.Input.PushKey(ConsoleKey.Enter);

        var choices = new[] { "Red", "Green", "Blue" };

        // Act
        var result = await logger.SelectAsync(
            "What is your favorite color?",
            choices,
            (choice) => choice,
            CancellationToken.None
        );

        // Assert
        Assert.Equal("Green", result);
        Assert.Equal(
            """
            ? What is your favorite color?
                                          
            > Red                         
              Green                       
              Blue                        ? What is your favorite color?
                                          
              Red                         
            > Green                       
              Blue                        ? What is your favorite color?
                                          
              Red                         
              Green                       
            > Blue                        ? What is your favorite color?
                                          
            > Red                         
              Green                       
              Blue                        ? What is your favorite color?
                                          
              Red                         
            > Green                       
              Blue                        ? What is your favorite color? Green

            """,
            _error.Output,
            ignoreLineEndingDifferences: true
        );
        Assert.Equal("", _out.Output);
    }
}
