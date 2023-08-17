namespace DotnetSops.CommandLine.Services.Sops;

internal interface ISopsService
{
    /// <summary>
    /// Encrypt file
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outoutFilePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="SopsMissingException"></exception>
    /// <exception cref="SopsExecutionException"></exception>
    Task EncryptAsync(FileInfo inputFilePath, FileInfo outoutFilePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypt file
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outoutFilePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="SopsMissingException"></exception>
    /// <exception cref="SopsExecutionException"></exception>
    Task DecryptAsync(FileInfo inputFilePath, FileInfo outoutFilePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Run command with secrets as environment variables
    /// </summary>
    /// <param name="command"></param>
    /// <param name="inputFilePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="SopsMissingException"></exception>
    /// <exception cref="SopsExecutionException"></exception>
    Task RunCommandWithSecretsEnvironmentAsync(string command, FileInfo inputFilePath, CancellationToken cancellationToken = default);
}
