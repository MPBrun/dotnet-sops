using System.CommandLine;

namespace DotSops.CommandLine.Commands;
internal class RootDotSopsCommand : CliRootCommand
{
    private readonly CliOption<bool> _verboseOption = new("--verbose")
    {
        Recursive = true,
        Description = Properties.Resources.RootDotSopsCommandVerboseOptionDescription,
    };

    public RootDotSopsCommand(Services.IServiceProvider serviceProvider)
        : base(Properties.Resources.RootCommandDescription)
    {
        Add(_verboseOption);

        Add(new InitializeCommand(serviceProvider));
        Add(new EncryptCommand(serviceProvider));
        Add(new DecryptCommand(serviceProvider));
        Add(new DownloadSopsCommand(serviceProvider));

        _verboseOption.Validators.Add(optionResult =>
        {
            var value = optionResult.GetValue(_verboseOption);
            serviceProvider.Verbose = value;
        });
    }
}

