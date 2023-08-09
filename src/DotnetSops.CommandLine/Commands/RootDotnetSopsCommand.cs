using System.CommandLine;
using DotnetSops.CommandLine.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSops.CommandLine.Commands;
internal class RootDotnetSopsCommand : CliRootCommand
{
    private readonly CliOption<bool> _verboseOption = new("--verbose")
    {
        Recursive = true,
        Description = Properties.Resources.RootDotnetSopsCommandVerboseOptionDescription,
    };

    public RootDotnetSopsCommand(IServiceProvider serviceProvider)
        : base(Properties.Resources.RootCommandDescription)
    {
        Add(_verboseOption);

        Add(new InitializeCommand(serviceProvider));
        Add(new EncryptCommand(serviceProvider));
        Add(new DecryptCommand(serviceProvider));
        Add(new RunCommand(serviceProvider));
        Add(new DownloadSopsCommand(serviceProvider));

        _verboseOption.Validators.Add(optionResult =>
        {
            var value = optionResult.GetValue(_verboseOption);
            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Verbose = value;
        });
    }
}
