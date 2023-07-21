using System.CommandLine;
using DotSops.CommandLine.Commands;
using DotSops.CommandLine.Services;

var serviceProvider = new ServiceProvider();
var rootCommand = new RootDotSopsCommand(serviceProvider);

var config = new CliConfiguration(rootCommand);

return await config.InvokeAsync(args);
