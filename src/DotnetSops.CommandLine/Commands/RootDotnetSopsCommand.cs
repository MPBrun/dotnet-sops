using System.CommandLine;

namespace DotnetSops.CommandLine.Commands;
internal class RootDotnetSopsCommand : CliRootCommand
{
    private readonly CliOption<bool> _verboseOption = new("--verbose")
    {
        Recursive = true,
        Description = Properties.Resources.RootDotnetSopsCommandVerboseOptionDescription,
    };

    public RootDotnetSopsCommand(Services.IServiceProvider serviceProvider)
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

