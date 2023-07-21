using System.CommandLine;
using System.CommandLine.Parsing;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;

namespace DotSops.CommandLine.Commands;

internal class DecryptCommand : CliCommand
{
    public const string CommandName = "decrypt";

    private readonly Services.IServiceProvider _serviceProvider;

    private readonly CliOption<string> _userSecretsIdOption = new("--id")
    {
        Description = Properties.Resources.DecryptCommandSecretsIdOptionDescription,
        Required = true
    };
    private readonly CliOption<FileInfo> _inputFileOption = new("--file")
    {
        Description = Properties.Resources.DecryptCommandFileOptionDescription,
        DefaultValueFactory = _ => new FileInfo("secrets.json")
    };

    public DecryptCommand(Services.IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.DecryptCommandDescription)
    {
        Add(_userSecretsIdOption);
        Add(_inputFileOption);

        _inputFileOption.Validators.Add(ValidateInputFile);
        _serviceProvider = serviceProvider;

        SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
              parseResult.GetValue(_userSecretsIdOption)!,
              parseResult.GetValue(_inputFileOption)!,
              _serviceProvider.SopsService.Value,
              _serviceProvider.UserSecretsService.Value,
              cancellationToken);
        });
    }

    private void ValidateInputFile(OptionResult optionResult)
    {
        var inputFile = optionResult.GetValue(_inputFileOption)!;
        if (!inputFile.Exists)
        {
            optionResult.AddError(LocalizationResources.FileDoesNotExist(inputFile.FullName));
        }
    }

    private static async Task ExecuteAsync(string userSecretId, FileInfo inputFile, ISopsService sopsService, IUserSecretsService userSecretsService, CancellationToken cancellationToken)
    {
        var outputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);

        if (outputFile.Directory != null)
        {
            Directory.CreateDirectory(outputFile.Directory.FullName);
        }

        await sopsService.DecryptAsync(inputFile, outputFile, cancellationToken);
    }
}
