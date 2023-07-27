namespace DotSops.CommandLine.Services.FileBom;
internal interface IFileBomService
{
    Task RemoveBomFromFileAsync(FileInfo file, CancellationToken cancellationToken = default);
}
