using System.CommandLine;

namespace DotSops.CommandLine.Commands;
internal class RootDotSopsCommand : RootCommand
{
    //private readonly Option<bool> _verboseOption = new("--verbose", "Tasdad.");
    /*
     *  https://learn.microsoft.com/en-us/dotnet/standard/commandline/syntax#the---verbosity-option
     * 
     *  Quiet
     *  Normal
     *  Diagnostic
     */

    public RootDotSopsCommand()
        : base(Properties.Resources.RootCommandDescription)
    {
        AddCommand(new EncryptCommand());
        AddCommand(new DecryptCommand());
        AddCommand(new DownloadSopsCommand());
    }
}
