namespace DotnetSops.CommandLine.Services.Sops;
internal class SopsReleaseFileInfo
{
    public required string ReleaseFileName { get; init; }

    public required string ExecutableFileName { get; init; }

    public required string Sha512Checksum { get; init; }
}
