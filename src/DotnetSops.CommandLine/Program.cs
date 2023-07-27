using System.CommandLine;
using DotnetSops.CommandLine.Commands;
using DotnetSops.CommandLine.Services;

var serviceProvider = new ServiceProvider();
var rootCommand = new RootDotnetSopsCommand(serviceProvider);

var config = new CliConfiguration(rootCommand);

return await config.InvokeAsync(args);
