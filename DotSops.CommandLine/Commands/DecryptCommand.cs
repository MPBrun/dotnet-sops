using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using DotSops.CommandLine.Binding;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;

namespace DotSops.CommandLine.Commands;

internal class DecryptCommand : Command
{
    public const string CommandName = "decrypt";

    private readonly Option<string> _userSecretsIdOption = new("--id", Properties.Resources.DecryptCommandSecretsIdOptionDescription) { IsRequired = true };
    private readonly Option<FileInfo> _inputFileOption = new("--file", () => new FileInfo("secrets.json"), Properties.Resources.DecryptCommandFileOptionDescription);

    public DecryptCommand()
        : base(CommandName, Properties.Resources.DecryptCommandDescription)
    {
        AddOption(_userSecretsIdOption);
        AddOption(_inputFileOption);

        _inputFileOption.AddValidator(ValidateInputFile);

        this.SetHandler(ExecuteAsync, _userSecretsIdOption, _inputFileOption, new InjectableBinder<InvocationContext>(), new InjectableBinder<ISopsService>(), new InjectableBinder<IUserSecretsService>());
    }

    private void ValidateInputFile(OptionResult optionResult)
    {
        var inputFile = optionResult.GetValueOrDefault<FileInfo>()!;
        if (!inputFile.Exists)
        {
            optionResult.ErrorMessage = LocalizationResources.FileDoesNotExist(inputFile.FullName);
        }
    }

    private static async Task ExecuteAsync(string userSecretId, FileInfo inputFile, InvocationContext invocationContext, ISopsService sopsService, IUserSecretsService userSecretsService)
    {
        var outputFile = userSecretsService.GetSecretsPathFromSecretsId(userSecretId);

        if (outputFile.Directory != null)
        {
            Directory.CreateDirectory(outputFile.Directory.FullName);
        }

        await sopsService.DecryptAsync(inputFile, outputFile, invocationContext.GetCancellationToken());
    }
}
