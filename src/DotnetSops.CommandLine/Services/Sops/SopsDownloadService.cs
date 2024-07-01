using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DotnetSops.CommandLine.Services.PlatformInformation;

namespace DotnetSops.CommandLine.Services.Sops;

internal class SopsDownloadService : ISopsDownloadService
{
    private const string Version = "v3.9.0";
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

        // Calculate file hash
        var hash = BitConverter
            .ToString(SHA256.HashData(content))
            .Replace("-", "", StringComparison.OrdinalIgnoreCase);

#pragma warning disable CA1308 // Normalize strings to uppercase
        // Lowercase to follow same format as defined by *.checksums.txt
        hash = hash.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

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

    // Checksums from https://github.com/getsops/sops/releases/download/v3.8.0/sops-v3.8.0.checksums.txt
    private SopsReleaseFileInfo PlatformSopsReleaseFile()
    {
        if (_platformInformation.IsWindows())
        {
            return new SopsReleaseFileInfo()
            {
                ReleaseFileName = $"sops-{Version}.exe",
                ExecutableFileName = "sops.exe",
                Sha256Checksum = "2b45084e9e6308fa465eeac2419d497b5b16b66d332af18c03feb3d68e51f52f",
            };
        }
        else if (_platformInformation.IsMacOS())
        {
            return _platformInformation.ProcessArchitecture switch
            {
                Architecture.Arm64
                    => new SopsReleaseFileInfo()
                    {
                        ReleaseFileName = $"sops-{Version}.darwin.arm64",
                        ExecutableFileName = "sops",
                        Sha256Checksum =
                            "72f9d59b825a20ac0019f370c18b3265608b1b0a271bc052f6007c45b95212fd",
                    },
                Architecture.X64
                    => new SopsReleaseFileInfo()
                    {
                        ReleaseFileName = $"sops-{Version}.darwin.amd64",
                        ExecutableFileName = "sops",
                        Sha256Checksum =
                            "25457955df3bd602b4fc15620d4574e79fbb554ada0211e85c6aca6eee2ba3ea",
                    },
                Architecture.X86
                    => new SopsReleaseFileInfo()
                    {
                        ReleaseFileName = $"sops-{Version}.darwin",
                        ExecutableFileName = "sops",
                        Sha256Checksum =
                            "aceb00e3b75fc86bfb1d8d267852e9c8f309cf65e18cb3cae8298ad3c06c590a",
                    },
                Architecture.Arm => throw new NotSupportedException(),
                Architecture.Wasm => throw new NotSupportedException(),
                Architecture.S390x => throw new NotSupportedException(),
                Architecture.LoongArch64 => throw new NotSupportedException(),
                Architecture.Armv6 => throw new NotSupportedException(),
                Architecture.Ppc64le => throw new NotSupportedException(),
                _ => throw new NotSupportedException(),
            };
        }
        else if (_platformInformation.IsLinux())
        {
            return _platformInformation.ProcessArchitecture switch
            {
                Architecture.Arm64
                    => new SopsReleaseFileInfo()
                    {
                        ReleaseFileName = $"sops-{Version}.linux.arm64",
                        ExecutableFileName = "sops",
                        Sha256Checksum =
                            "596f26de6d4f7d1cc44f9e27bfea3192ef77f810f31f3f4132a417860ab91ebc",
                    },
                Architecture.X64
                    => new SopsReleaseFileInfo()
                    {
                        ReleaseFileName = $"sops-{Version}.linux.amd64",
                        ExecutableFileName = "sops",
                        Sha256Checksum =
                            "0d65660fbe785647ff4f1764d7f69edf598f79d6d79ebbef4a501909b6ff6b82",
                    },
                Architecture.X86 => throw new NotSupportedException(),
                Architecture.Arm => throw new NotSupportedException(),
                Architecture.Wasm => throw new NotSupportedException(),
                Architecture.S390x => throw new NotSupportedException(),
                Architecture.LoongArch64 => throw new NotSupportedException(),
                Architecture.Armv6 => throw new NotSupportedException(),
                Architecture.Ppc64le => throw new NotSupportedException(),
                _ => throw new NotSupportedException(),
            };
        }
        throw new NotSupportedException(Properties.Resources.OperatingSystemNotSupported);
    }
}
