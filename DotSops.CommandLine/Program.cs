using System.CommandLine;
using System.CommandLine.Parsing;
using DotSops.CommandLine.Commands;
using DotSops.CommandLine.Services.FileBom;
using DotSops.CommandLine.Services.PlatformInformation;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;
using Spectre.Console;

var rootCommand = new RootDotSopsCommand();
var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseHelp((help) =>
    {
        //context.Output.WriteLine();
    })
    .UseExceptionHandler((exception, context) =>
    {
        if (exception is not OperationCanceledException)
        {
            var console = context.BindingContext.GetService(typeof(IAnsiConsole)) as IAnsiConsole;
            console?.MarkupLine($"[red]{exception.Message}[/]");
#if DEBUG
            console?.WriteLine();
            console?.WriteException(exception, ExceptionFormats.ShortenEverything);
#endif
        }
        context.ExitCode = 1;
    });
builder.AddMiddleware(s =>
{
    s.BindingContext.AddService(typeof(ISopsService), sp => new SopsService());
    s.BindingContext.AddService(typeof(IUserSecretsService), sp => new UserSecretsService());
    s.BindingContext.AddService(typeof(IFileBomService), sp => new FileBomService());
    s.BindingContext.AddService(typeof(ISopsDownloadService), sp => new SopsDownloadService(new PlatformInformationService(), new HttpClient()));
    s.BindingContext.AddService(typeof(IAnsiConsole), sp => AnsiConsole.Create(new AnsiConsoleSettings()
    {
        Ansi = AnsiSupport.Detect,
        ColorSystem = ColorSystemSupport.Detect,
        Out = new AnsiConsoleOutput(Console.Error),
    }));
});
var result = await builder.Build().InvokeAsync(args);
return result;
