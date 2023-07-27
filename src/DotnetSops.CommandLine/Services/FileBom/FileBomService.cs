namespace DotnetSops.CommandLine.Services.FileBom;
internal class FileBomService : IFileBomService
{
    public static readonly byte[] Bom = new byte[] { 0xEF, 0xBB, 0xBF };

    public async Task RemoveBomFromFileAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        var isBomFile = await IsBomFileAsync(file, cancellationToken);
        if (isBomFile)
        {
            // Read and write content to same file without bom as default.
            var content = await File.ReadAllTextAsync(file.FullName, cancellationToken);
            await File.WriteAllTextAsync(file.FullName, content, cancellationToken);
        }
    }

    private static async Task<bool> IsBomFileAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        using var stream = file.Open(FileMode.Open);
        var buffer = new byte[3].AsMemory();
        var readBytes = await stream.ReadAsync(buffer, cancellationToken);
        return readBytes == buffer.Length && buffer.Span.SequenceEqual(Bom);
    }
}
