using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DotnetSops.CommandLine.Services.PlatformInformation;

namespace DotnetSops.CommandLine.Services.Sops;

internal class SopsDownloadService : ISopsDownloadService
{
    private const string Version = "v3.10.2";
    private readonly IPlatformInformationService _platformInformation;
    private readonly HttpClient _httpClient;
    private readonly ISopsPathService _sopsPathService;
    private readonly ILogger _logger;

    public SopsDownloadService(
        IPlatformInformationService platformInformation,
        HttpClient httpClient,
        ISopsPathService sopsPathService,
        ILogger logger
    )
    {
        _platformInformation = platformInformation;
        _httpClient = httpClient;
        _sopsPathService = sopsPathService;
        _logger = logger;
    }

    public async Task DownloadAsync(CancellationToken cancellationToken = default)
    {
        var release = PlatformSopsReleaseFile();

        // Download sops
        var platformFile = release.ReleaseFileName;
        var url = new Uri(
            $"https://github.com/getsops/sops/releases/download/{Version}/{platformFile}"
        );

        _logger.LogDebug($"Download SOPS from '{url}'");

        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new SopsDownloadException(
                LocalizationResources.SopsDownloadHttpFailed((int)response.StatusCode, url)
            );
        }

        // Get content
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);

#if NET9_0_OR_GREATER
        var hash = Convert.ToHexStringLower(SHA256.HashData(content));
#else
        // Calculate file hash
        var hash = Convert.ToHexString(SHA256.HashData(content));

#pragma warning disable CA1308 // Normalize strings to uppercase
        // Lowercase to follow same format as defined by *.checksums.txt
        hash = hash.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
#endif

        // Verify content hash
        if (!hash.Equals(release.Sha256Checksum, StringComparison.OrdinalIgnoreCase))
        {
            throw new SopsDownloadException(
                LocalizationResources.SopsDownloadSha256Failed(release.Sha256Checksum, hash)
            );
        }

        // Save content to disk
        var localSopsDirectory = _sopsPathService.GetDotnetSopsUserDirectory();
        _ = Directory.CreateDirectory(localSopsDirectory);
        var destinationFileName = Path.Join(localSopsDirectory, release.ExecutableFileName);
        await File.WriteAllBytesAsync(destinationFileName, content, cancellationToken);
        if (!OperatingSystem.IsWindows())
        {
            File.SetUnixFileMode(
                destinationFileName,
                File.GetUnixFileMode(destinationFileName) | UnixFileMode.UserExecute
            );
        }
    }

    // Checksums from https://github.com/getsops/sops/releases/download/v3.9.4/sops-v3.9.4.checksums.txt
    private SopsReleaseFileInfo PlatformSopsReleaseFile()
    {
        if (_platformInformation.IsWindows())
        {
            return new SopsReleaseFileInfo()
            {
                ReleaseFileName = $"sops-{Version}.exe",
                ExecutableFileName = "sops.exe",
                Sha256Checksum = "bee270926fc55b5b89ed9ce87fb2569a36c74e99d63e6392090b3d0f0c2775eb",
            };
        }
        else if (_platformInformation.IsMacOS())
        {
            return _platformInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin.arm64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "51ee2c3ec2c4331cfe1c0c25168e1c4c8036900842700b9bb074dda92a6017f2",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "f48d73efc278326e54d0e6a056b285fd8f5f28549b19aff9b0fedbbdd846b20c",
                },
                Architecture.X86 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "64e77e81befb4a3fed9c10ef5b7643446d0d3e75183b829c2e7746f9c91ae8d4",
                },
                Architecture.Arm => throw new NotSupportedException(),
                Architecture.Wasm => throw new NotSupportedException(),
                Architecture.S390x => throw new NotSupportedException(),
                Architecture.LoongArch64 => throw new NotSupportedException(),
                Architecture.Armv6 => throw new NotSupportedException(),
                Architecture.Ppc64le => throw new NotSupportedException(),
#if NET9_0_OR_GREATER
                Architecture.RiscV64 => throw new NotSupportedException(),
#endif
                _ => throw new NotSupportedException(),
            };
        }
        else if (_platformInformation.IsLinux())
        {
            return _platformInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.linux.arm64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "16564c6b181d88505d9e0dfef62771894293d85cde5884d9b1a843859eee174b",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.linux.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "5488e32bc471de7982ad895dd054bbab3ab91c417a118426134551e9626e4e85",
                },
                Architecture.X86 => throw new NotSupportedException(),
                Architecture.Arm => throw new NotSupportedException(),
                Architecture.Wasm => throw new NotSupportedException(),
                Architecture.S390x => throw new NotSupportedException(),
                Architecture.LoongArch64 => throw new NotSupportedException(),
                Architecture.Armv6 => throw new NotSupportedException(),
                Architecture.Ppc64le => throw new NotSupportedException(),
#if NET9_0_OR_GREATER
                Architecture.RiscV64 => throw new NotSupportedException(),
#endif
                _ => throw new NotSupportedException(),
            };
        }
        throw new NotSupportedException(Properties.Resources.OperatingSystemNotSupported);
    }
}
