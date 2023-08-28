using DotnetSops.CommandLine.Prompts;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Prompts;
public class AskPromptTests
{
    [Fact]
    public void Show_Valid_ShowPrompt()
    {
        // Arrange
        var question = "What is the meaning of life?";
        var prompt = new AskPrompt(question);

        using var ansiConsole = new TestConsole();
        ansiConsole.Profile.Capabilities.Interactive = true;
        ansiConsole.Profile.Width = 1000;

        ansiConsole.Input.PushTextWithEnter("42");

        // Act
        var result = prompt.Show(ansiConsole);

        // Assert
        Assert.Equal("42", result);
        Assert.Equal("What is the meaning of life? 42\n", ansiConsole.Output);
    }

    [Fact]
    public async Task ShowAsync_Valid_ShowPrompt()
    {
        // Arrange
        var question = "What is the meaning of life?";
        var prompt = new AskPrompt(question);

        using var ansiConsole = new TestConsole();
        ansiConsole.Profile.Capabilities.Interactive = true;
        ansiConsole.Profile.Width = 1000;

        ansiConsole.Input.PushTextWithEnter("42");

        // Act
        var result = await prompt.ShowAsync(ansiConsole, CancellationToken.None);

        // Assert
        Assert.Equal("42", result);
        Assert.Equal("What is the meaning of life? 42\n", ansiConsole.Output);
    }
}
