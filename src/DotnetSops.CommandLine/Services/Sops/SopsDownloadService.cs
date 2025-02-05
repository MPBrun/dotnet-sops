using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DotnetSops.CommandLine.Services.PlatformInformation;

namespace DotnetSops.CommandLine.Services.Sops;

internal class SopsDownloadService : ISopsDownloadService
{
    private const string Version = "v3.9.4";
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

    // Checksums from https://github.com/getsops/sops/releases/download/v3.9.1/sops-v3.9.1.checksums.txt
    private SopsReleaseFileInfo PlatformSopsReleaseFile()
    {
        if (_platformInformation.IsWindows())
        {
            return new SopsReleaseFileInfo()
            {
                ReleaseFileName = $"sops-{Version}.exe",
                ExecutableFileName = "sops.exe",
                Sha256Checksum = "745ab6aa6d6e3fbbb8a3484ec22caf2cbf61b5f70d1416eea5d2a644de722f31",
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
                        "0ab2cc54f0d8b8414dab83c18017ae3c005d405480b343c788a84fa4af5b19e3",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "740b3a356c965830272d8f7aec2ea2e500d4a39f03e4aad347c004f3a0bdc2de",
                },
                Architecture.X86 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "de1afb0c4aed4829f775763beedec7622e44a21475cdb86ed8ee1cbcfdf43b98",
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
                        "bc946fef11dbe199587adac567037b69374c4202f928ca138443539efc85b357",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.linux.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "cd795109851c3a483bbaa66d15d19ddb2227ac0352b39e25d96b67d51edb6225",
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
