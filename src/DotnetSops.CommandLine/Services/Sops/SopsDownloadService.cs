using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DotnetSops.CommandLine.Services.PlatformInformation;

namespace DotnetSops.CommandLine.Services.Sops;

internal class SopsDownloadService : ISopsDownloadService
{
    private const string Version = "v3.11.0";
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

    // Checksums from https://github.com/getsops/sops/releases/download/v3.10.2/sops-v3.10.2.checksums.txt
    private SopsReleaseFileInfo PlatformSopsReleaseFile()
    {
        if (_platformInformation.IsWindows())
        {
            return _platformInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.arm64.exe",
                    ExecutableFileName = "sops.exe",
                    Sha256Checksum =
                        "9e08c708147634f485f8574a22add98b6a092511e84ff69c6d2849834aec865d",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.amd64.exe",
                    ExecutableFileName = "sops.exe",
                    Sha256Checksum =
                        "056d18d9f12966aebd33a8181b54c358bcb312661fadc5a3141bb6f84b9c3502",
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
        else if (_platformInformation.IsMacOS())
        {
            return _platformInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin.arm64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "99702df79737162b986641afb8d98251acb16a52e6cab556a6b6f57c608c059a",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "dece9b0131af5ced0f8c278a53c0cf06a4f0d1d70a177c0979f6d111654397ce",
                },
                Architecture.X86 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "c2a97277390760595265a7401e36e9989debf62420551d98debc1b054ef51d82",
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
                        "e91ddc04e6a78f5aed9e4fc347a279b539c43b74d99e6b8078e2f2f6f5b309f5",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.linux.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "79b0f844237bd4b0446e4dc884dbc1765fc7dedc3968f743d5949c6f2e701739",
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
