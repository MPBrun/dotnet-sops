using Spectre.Console;

namespace DotnetSops.CommandLine.Prompts;
internal class ConfirmationPrompt : IPrompt<bool>
{
    private readonly string _prompt;

    public char Yes { get; set; } = 'y';

    public char No { get; set; } = 'n';

    public string InvalidChoiceMessage { get; set; } = "[red]Please select one of the available options[/]";

    public string PromptStyle { get; set; } = "blue";

    public string ChoicesStyle { get; set; } = "gray";

    public bool ShowChoices { get; set; } = true;

    public bool ShowDefaultValue { get; set; } = true;

    public StringComparer Comparer { get; set; } = StringComparer.CurrentCultureIgnoreCase;

    public ConfirmationPrompt(string prompt)
    {
        _prompt = prompt;
    }

    public bool Show(IAnsiConsole console)
    {
        var prompt = CreatePrompt();

        var result = prompt.Show(console);

        return Comparer.Compare(Yes.ToString(), result.ToString()) == 0;
    }

    public async Task<bool> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken)
    {
        var prompt = CreatePrompt();

        var result = await prompt.ShowAsync(console, cancellationToken);

        return Comparer.Compare(Yes.ToString(), result.ToString()) == 0;
    }

    private TextPrompt<char> CreatePrompt()
    {
        return new TextPrompt<char>(_prompt, Comparer)
            .InvalidChoiceMessage(InvalidChoiceMessage)
            .ValidationErrorMessage(InvalidChoiceMessage)
            .ShowChoices(ShowChoices)
            .ShowDefaultValue(ShowDefaultValue)
            .PromptStyle(PromptStyle)
            .ChoicesStyle(ChoicesStyle)
            .AddChoice(Yes)
            .AddChoice(No);
    }
}
