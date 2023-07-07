using System.CommandLine;
using System.CommandLine.Invocation;
using DotSops.CommandLine.Binding;
using DotSops.CommandLine.Services.FileBom;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;

namespace DotSops.CommandLine.Commands;

internal class EncryptCommand : Command
{
    public const string CommandName = "encrypt";

    private readonly Option<string> _userSecretsIdOption = new("--id", Properties.Resources.EncryptCommandSecretsIdOptionDescription) { IsRequired = true };
    private readonly Option<FileInfo> _outputFileOption = new("--file", () => new FileInfo("secrets.json"), Properties.Resources.EncryptCommandFileOptionDescription);

    public EncryptCommand()
        : base(CommandName, Properties.Resources.EncryptCommandDescription)
    {
        AddOption(_userSecretsIdOption);
        AddOption(_outputFileOption);

        // _userSecretsIdOption.AddValidator(ValidateUserSecretId);

        this.SetHandler(ExecuteAsync, _userSecretsIdOption, _outputFileOption, new InjectableBinder<InvocationContext>(), new InjectableBinder<ISopsService>(), new InjectableBinder<IUserSecretsService>(), new InjectableBinder<IFileBomService>());
    }

    //private void ValidateUserSecretId(OptionResult optionResult)
    //{
    //    var userSecretId = optionResult.GetValueOrDefault<string>()!;
    //    var inputFile = _userSecretsService.GetSecretsPathFromSecretsId(userSecretId);
    //    if (!inputFile.Exists)
    //    {
    //        optionResult.ErrorMessage = LocalizationResources.UserSecretsFileDoesNotExist(inputFile.FullName);
    //    }
    //}

    private static async Task ExecuteAsync(string userSecretId, FileInfo outputFile, InvocationContext invocationContext, ISopsService sopsService, IUserSecretsService userSecretsService, IFileBomService fileBomService)
    {
        var cancellationToken = invocationContext.GetCancellationToken();
        var inputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);
        if (!inputFile.Exists)
        {
            throw new DotSopsException(LocalizationResources.UserSecretsFileDoesNotExist(inputFile.FullName));
        }

        await fileBomService.RemoveBomFromFileAsync(inputFile, cancellationToken);

        await sopsService.EncryptAsync(inputFile, outputFile, cancellationToken);
    }
}
