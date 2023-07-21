using System.CommandLine;

namespace DotSops.CommandLine.Commands;
internal class RootDotSopsCommand : CliRootCommand
{
    private readonly CliOption<string> _verboseOption = new("--verbose");
    /*
     *  https://learn.microsoft.com/en-us/dotnet/standard/commandline/syntax#the---verbosity-option
     * 
     *  Quiet
     *  Normal
     *  Diagnostic
     */

    public RootDotSopsCommand(Services.IServiceProvider serviceProvider)
        : base(Properties.Resources.RootCommandDescription)
    {
        _verboseOption.AcceptOnlyFromAmong("quiet", "normal", "diagnostic");
        Add(_verboseOption);
        Add(new EncryptCommand(serviceProvider));
        Add(new DecryptCommand(serviceProvider));
        Add(new DownloadSopsCommand(serviceProvider));
    }
}
