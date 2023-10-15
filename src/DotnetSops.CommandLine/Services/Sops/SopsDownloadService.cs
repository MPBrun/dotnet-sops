using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DotnetSops.CommandLine.Services.PlatformInformation;

namespace DotnetSops.CommandLine.Services.Sops;
internal class SopsDownloadService : ISopsDownloadService
{
    private const string Version = "v3.8.1";
    private readonly IPlatformInformationService _platformInformation;
    private readonly HttpClient _httpClient;
    private readonly ISopsPathService _sopsPathService;
    private readonly ILogger _logger;

    public SopsDownloadService(IPlatformInformationService platformInformation, HttpClient httpClient, ISopsPathService sopsPathService, ILogger logger)
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
        var url = new Uri($"https://github.com/getsops/sops/releases/download/v{Version}/{platformFile}");

        _logger.LogDebug($"Download SOPS from '{url}'");

        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new SopsDownloadException(LocalizationResources.SopsDownloadHttpFailed((int)response.StatusCode, url));
        }

        // Get content
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        // Calculate file hash
        var hash = BitConverter.ToString(SHA256.HashData(content))
            .Replace("-", "", StringComparison.OrdinalIgnoreCase);

#pragma warning disable CA1308 // Normalize strings to uppercase
        // Lowercase to follow same format as defined by *.checksums.txt
        hash = hash.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

        // Verify content hash
        if (!hash.Equals(release.Sha256Checksum, StringComparison.OrdinalIgnoreCase))
        {
            throw new SopsDownloadException(LocalizationResources.SopsDownloadSha256Failed(release.Sha256Checksum, hash));
        }

        // Save content to disk
        var localSopsDirectory = _sopsPathService.GetDotnetSopsUserDirectory();
        Directory.CreateDirectory(localSopsDirectory);
        var destinationFileName = Path.Join(localSopsDirectory, release.ExecutableFileName);
        await File.WriteAllBytesAsync(destinationFileName, content, cancellationToken);
        if (!OperatingSystem.IsWindows())
        {
            File.SetUnixFileMode(destinationFileName, File.GetUnixFileMode(destinationFileName) | UnixFileMode.UserExecute);
        }
    }

    // Checksums from https://github.com/getsops/sops/releases/download/v3.8.0/sops-v3.8.0.checksums.txt
    private SopsReleaseFileInfo PlatformSopsReleaseFile()
    {
        if (_platformInformation.IsWindows())
        {
            return new SopsReleaseFileInfo()
            {
                ReleaseFileName = $"sops-v{Version}.exe",
                ExecutableFileName = "sops.exe",
                Sha256Checksum = "8bb627307ddefbc529ab844c7bddbc71ae3ba3643a919cd6b9e127dc74cc1841",
            };
        }
        else if (_platformInformation.IsMacOS())
        {
            return _platformInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.darwin.arm64",
                    ExecutableFileName = "sops",
                    Sha256Checksum = "44d98ffd8622629adab069f5ad30ccada702c6cff11102f8be932f98cd04ae42",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.darwin.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum = "f1b2fb34014a3965c5aad9029986fa3499419675c8344b3dab151f9efb8a3b88",
                },
                Architecture.X86 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.darwin",
                    ExecutableFileName = "sops",
                    Sha256Checksum = "b9a686515fea2919402077c5cacc1547bd7d8467d4d915830657cbd25ce6098a",
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
                Architecture.Arm64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.linux.arm64",
                    ExecutableFileName = "sops",
                    Sha256Checksum = "5ec31eaed635e154b59ff4b7c9b311b6e616bd4818a68899c2f9db00c81e3a63",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.linux.amd64",
                    ExecutableFileName = "sops",
                    Sha256Checksum = "48fb4a6562014a9213be15b4991931266d040b9b64dba8dbcd07b902e90025c0",
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
