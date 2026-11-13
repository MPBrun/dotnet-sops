using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DotnetSops.CommandLine.Services.PlatformInformation;

namespace DotnetSops.CommandLine.Services.Sops;

internal class SopsDownloadService : ISopsDownloadService
{
    private const string Version = "v3.13.3";
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

        var hash = Convert.ToHexStringLower(SHA256.HashData(content));

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

    // Checksums from https://github.com/getsops/sops/releases/download/v3.13.3/sops-v3.13.3.checksums.txt
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
                        "7f936a0d08edbeba3eb8f50bad1fe71eba83a140a80dfd17051f3bdb4facb08a",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.amd64.exe",
                    ExecutableFileName = "sops.exe",
                    Sha256Checksum =
                        "a4a9a398858fe8b2ef72d9686d930bf7c5cece9be74ad83ac3b53cfdd70e6b1c",
                },
                Architecture.X86 => throw new NotSupportedException(),
                Architecture.Arm => throw new NotSupportedException(),
                Architecture.Wasm => throw new NotSupportedException(),
                Architecture.S390x => throw new NotSupportedException(),
                Architecture.LoongArch64 => throw new NotSupportedException(),
                Architecture.Armv6 => throw new NotSupportedException(),
                Architecture.Ppc64le => throw new NotSupportedException(),
                Architecture.RiscV64 => throw new NotSupportedException(),
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
                        "b97c0d434aab577dc40310e8d22ff9e45eef4c80638ab978daae9b4681c59286",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "42162d5cef10b74fcf80a045a70e658d7ce6e63d6ea1be6f347e44015714468d",
                },
                Architecture.X86 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.darwin",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "ada49efb072e8d02434b15cdd4746561b7b37e82813ce11efe5cb311439f288b",
                },
                Architecture.Arm => throw new NotSupportedException(),
                Architecture.Wasm => throw new NotSupportedException(),
                Architecture.S390x => throw new NotSupportedException(),
                Architecture.LoongArch64 => throw new NotSupportedException(),
                Architecture.Armv6 => throw new NotSupportedException(),
                Architecture.Ppc64le => throw new NotSupportedException(),
                Architecture.RiscV64 => throw new NotSupportedException(),
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
                        "53b0abacd38ef1b12a66d6c100956691b9cefce018d91f81e73ddf7438b94d77",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-{Version}.linux.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum =
                        "e5bec3346a873ae91d871550f3e698c1aad962aff462a080e40f25fde17fef6b",
                },
                Architecture.X86 => throw new NotSupportedException(),
                Architecture.Arm => throw new NotSupportedException(),
                Architecture.Wasm => throw new NotSupportedException(),
                Architecture.S390x => throw new NotSupportedException(),
                Architecture.LoongArch64 => throw new NotSupportedException(),
                Architecture.Armv6 => throw new NotSupportedException(),
                Architecture.Ppc64le => throw new NotSupportedException(),
                Architecture.RiscV64 => throw new NotSupportedException(),
                _ => throw new NotSupportedException(),
            };
        }
        throw new NotSupportedException(Properties.Resources.OperatingSystemNotSupported);
    }
}
