using System.ComponentModel;
using System.Diagnostics;

namespace DotnetSops.CommandLine.Services.Sops;

internal class SopsService : ISopsService
{
    private readonly ILogger _logger;

    public async Task EncryptAsync(FileInfo inputFilePath, FileInfo outoutFilePath, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(new string[] { "--output", outoutFilePath.FullName, "--encrypt", inputFilePath.FullName }, cancellationToken);
    }

    public async Task DecryptAsync(FileInfo inputFilePath, FileInfo outoutFilePath, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(new string[] { "--output", outoutFilePath.FullName, "--decrypt", inputFilePath.FullName }, cancellationToken);
    }

    public async Task RunCommandWithSecretsEnvironmentAsync(string command, FileInfo inputFilePath, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(new string[] { "exec-env", inputFilePath.FullName, command }, cancellationToken);
    }

    public SopsService(ILogger logger)
    {
        _logger = logger;
    }

    private async Task ExecuteAsync(string[] arguments, CancellationToken cancellationToken = default)
    {
        var processStartInfo = new ProcessStartInfo()
        {
            WorkingDirectory = Directory.GetCurrentDirectory(),
            FileName = "sops",
        };
        foreach (var argument in arguments)
        {
            processStartInfo.ArgumentList.Add(argument);
        }

        try
        {
            _logger.LogDebug($"Executing sops with arguments: {string.Join(" ", processStartInfo.ArgumentList)}");

            using var process = Process.Start(processStartInfo) ?? throw new SopsMissingException(Properties.Resources.SopsRunFailed);
            await process.WaitForExitAsync(cancellationToken);
            if (process.ExitCode != 0)
            {
                throw new SopsExecutionException(Properties.Resources.SopsRunFailedWithError)
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
