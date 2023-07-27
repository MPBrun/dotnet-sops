namespace DotSops.CommandLine.Services.Sops;

internal interface ISopsService
{
    Task EncryptAsync(FileInfo inputFilePath, FileInfo outoutFilePath, CancellationToken cancellationToken = default);

    Task DecryptAsync(FileInfo inputFilePath, FileInfo outoutFilePath, CancellationToken cancellationToken = default);
}
