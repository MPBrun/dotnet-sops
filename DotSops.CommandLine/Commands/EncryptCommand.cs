using System.CommandLine;
using System.CommandLine.Parsing;
using DotSops.CommandLine.Services.FileBom;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;

namespace DotSops.CommandLine.Commands;

internal partial class EncryptCommand : CliCommand
{
    public const string CommandName = "encrypt";

    private readonly Services.IServiceProvider _serviceProvider;

    private readonly CliOption<string> _userSecretsIdOption = new("--id")
    {
        Description = Properties.Resources.EncryptCommandSecretsIdOptionDescription,
        Required = true,
    };
    private readonly CliOption<FileInfo> _outputFileOption = new("--file")
    {
        Description = Properties.Resources.EncryptCommandFileOptionDescription,
        DefaultValueFactory = (_) => new FileInfo("secrets.json"),
    };

    public EncryptCommand(Services.IServiceProvider serviceProvider)
        : base(CommandName, Properties.Resources.EncryptCommandDescription)
    {
        _serviceProvider = serviceProvider;

        Add(_userSecretsIdOption);
        Add(_outputFileOption);

        _outputFileOption.Validators.Add(ValidateUserSecretId);

        SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
              parseResult.GetValue(_userSecretsIdOption)!,
              parseResult.GetValue(_outputFileOption)!,
              _serviceProvider.SopsService.Value,
              _serviceProvider.UserSecretsService.Value,
              _serviceProvider.FileBomService.Value,
              cancellationToken);
        });
    }

    private void ValidateUserSecretId(OptionResult optionResult)
    {
        var userSecretId = optionResult.GetValue(_userSecretsIdOption)!;
        var inputFile = _serviceProvider.UserSecretsService.Value.GetSecretsPathFromSecretsId(userSecretId);
        if (!inputFile.Exists)
        {
            optionResult.AddError(LocalizationResources.UserSecretsFileDoesNotExist(inputFile.FullName));
        }
    }

    private static async Task ExecuteAsync(string userSecretId, FileInfo outputFile, ISopsService sopsService, IUserSecretsService userSecretsService, IFileBomService fileBomService, CancellationToken cancellationToken)
    {
        var inputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);
        if (!inputFile.Exists)
        {
            throw new DotSopsException(LocalizationResources.UserSecretsFileDoesNotExist(inputFile.FullName));
        }

        await fileBomService.RemoveBomFromFileAsync(inputFile, cancellationToken);

        await sopsService.EncryptAsync(inputFile, outputFile, cancellationToken);
    }
}
