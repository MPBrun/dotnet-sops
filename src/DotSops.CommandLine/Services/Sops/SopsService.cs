using System.ComponentModel;
using System.Diagnostics;

namespace DotSops.CommandLine.Services.Sops;

internal class SopsService : ISopsService
{
    public string WorkingDirectory { get; }

    public async Task EncryptAsync(FileInfo inputFilePath, FileInfo outoutFilePath, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(new string[] { "--output", outoutFilePath.FullName, "--encrypt", inputFilePath.FullName, "--verbose" }, cancellationToken);
    }

    public async Task DecryptAsync(FileInfo inputFilePath, FileInfo outoutFilePath, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(new string[] { "--output", outoutFilePath.FullName, "--decrypt", inputFilePath.FullName, "--verbose" }, cancellationToken);
    }

    public SopsService()
        : this(Directory.GetCurrentDirectory())
    {

    }

    internal SopsService(string workingDirectory)
    {
        WorkingDirectory = workingDirectory;
    }

    private async Task ExecuteAsync(string[] arguments, CancellationToken cancellationToken = default)
    {
        var processStartInfo = new ProcessStartInfo()
        {
            WorkingDirectory = WorkingDirectory,
            FileName = "sops",
            RedirectStandardError = true,
        };
        foreach (var argument in arguments)
        {
            processStartInfo.ArgumentList.Add(argument);
        }

        try
        {
            using var process = Process.Start(processStartInfo) ?? throw new SopsMissingException(Properties.Resources.SopsRunFailed);
            await process.WaitForExitAsync(cancellationToken);
            if (process.ExitCode != 0)
            {
                var output = await process.StandardError.ReadToEndAsync(cancellationToken);
                throw new SopsExecutionException(LocalizationResources.SopsRunFailedWithError(output.ReplaceLineEndings().Trim()))
                {
                    ExitCode = process.ExitCode
                };
            }
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 2)
        {
            throw new SopsMissingException(Properties.Resources.SopsIsMissing);
        }
    }
}
