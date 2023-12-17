using DotnetSops.CommandLine.Prompts;
using Spectre.Console.Testing;

namespace DotnetSops.CommandLine.Tests.Prompts;

public class ConfirmationPromptTests
{
    [Fact]
    public void Show_ValidTrue_ShowPrompt()
    {
        // Arrange
        var question = "Are you ok?";
        var prompt = new ConfirmationPrompt(question);

        using var ansiConsole = new TestConsole();
        ansiConsole.Profile.Capabilities.Interactive = true;
        ansiConsole.Profile.Width = 1000;

        ansiConsole.Input.PushTextWithEnter("y");

        // Act
        var result = prompt.Show(ansiConsole);

        // Assert
        Assert.True(result);
        Assert.Equal("Are you ok? [y/n] (n): y\n", ansiConsole.Output);
    }

    [Fact]
    public async Task ShowAsync_ValidFalse_ShowPrompt()
    {
        // Arrange
        var question = "Are you ok?";
        var prompt = new ConfirmationPrompt(question);

        using var ansiConsole = new TestConsole();
        ansiConsole.Profile.Capabilities.Interactive = true;
        ansiConsole.Profile.Width = 1000;

        ansiConsole.Input.PushTextWithEnter("n");

        // Act
        var result = await prompt.ShowAsync(ansiConsole, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Equal("Are you ok? [y/n] (n): n\n", ansiConsole.Output);
    }

    [Fact]
    public void Show_Invalid_ShowInfo()
    {
        // Arrange
        var question = "Are you ok?";
        var prompt = new ConfirmationPrompt(question);

        using var ansiConsole = new TestConsole();
        ansiConsole.Profile.Capabilities.Interactive = true;
        ansiConsole.Profile.Width = 1000;

        ansiConsole.Input.PushTextWithEnter("q");
        ansiConsole.Input.PushTextWithEnter("y");

        // Act
        var result = prompt.Show(ansiConsole);

        // Assert
        Assert.True(result);
        Assert.Equal(
            """
            Are you ok? [y/n] (n): q
            Please select one of the available options
            Are you ok? [y/n] (n): y

            """,
            ansiConsole.Output,
            ignoreLineEndingDifferences: true
        );
    }
}
