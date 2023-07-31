using Spectre.Console;

namespace DotnetSops.CommandLine.Prompts;
internal class AskPrompt : IPrompt<string>
{
    private readonly string _prompt;

    public string PromptStyle { get; set; } = "blue";

    public AskPrompt(string prompt)
    {
        _prompt = prompt;
    }

    public string Show(IAnsiConsole console)
    {
        var prompt = CreatePrompt();
        return prompt.Show(console);
    }

    public async Task<string> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken)
    {
        var prompt = CreatePrompt();
        return await prompt.ShowAsync(console, cancellationToken);
    }

    private TextPrompt<string> CreatePrompt()
    {
        return new TextPrompt<string>(_prompt)
            .PromptStyle(PromptStyle);
    }
}
