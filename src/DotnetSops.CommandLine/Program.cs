using System.CommandLine;
using DotnetSops.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Services.PlatformInformation;
using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Services.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

var serviceProvider = new ServiceCollection()
    .AddSingleton<ISopsService, SopsService>()
    .AddSingleton<IUserSecretsService, UserSecretsService>()
    .AddSingleton<IFileBomService, FileBomService>()
    .AddSingleton<ISopsDownloadService, SopsDownloadService>()
    .AddSingleton<ISopsPathService, SopsPathService>()
    .AddSingleton<IPlatformInformationService, PlatformInformationService>()
    .AddSingleton<IProjectInfoService, ProjectInfoService>()
    .AddSingleton(_ => new HttpClient())
    .AddSingleton<ILogger>(_ =>
    {
        var errorConsole = AnsiConsole.Create(
            new AnsiConsoleSettings()
            {
                Ansi = AnsiSupport.Detect,
                ColorSystem = ColorSystemSupport.Detect,
                Out = new AnsiConsoleOutput(Console.Error),
            }
        );

        var outConsole = AnsiConsole.Create(
            new AnsiConsoleSettings()
            {
                Ansi = AnsiSupport.Detect,
                ColorSystem = ColorSystemSupport.Detect,
                Out = new AnsiConsoleOutput(Console.Out),
            }
        );

        return new Logger(outConsole, errorConsole);
    })
    .BuildServiceProvider();

var rootCommand = new RootDotnetSopsCommand(serviceProvider);

var config = new InvocationConfiguration() { Output = new ReplaceUsageHelpTextWriter(Console.Out) };

return await rootCommand.Parse(args).InvokeAsync(config);
