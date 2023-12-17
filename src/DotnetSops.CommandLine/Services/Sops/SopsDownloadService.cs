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
        Directory.CreateDirectory(localSopsDirectory);
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
                Sha256Checksum = "fe1f6299294b47ceda565e1091e843ee3f3db58764901d4298eb00558189e25f",
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
                            "b5d172960c135c7b8cd9719cee94283bccdf5c046c7563391eee8dd4882d6573",
                    },
                Architecture.X64
                    => new SopsReleaseFileInfo()
                    {
                        ReleaseFileName = $"sops-{Version}.darwin.amd64",
                        ExecutableFileName = "sops",
                        Sha256Checksum =
                            "aa3e79c1ff7a923d380b95b01fb0bc84ae1f5209b0a149b3f4c1936037792330",
                    },
                Architecture.X86
                    => new SopsReleaseFileInfo()
                    {
                        ReleaseFileName = $"sops-{Version}.darwin",
                        ExecutableFileName = "sops",
                        Sha256Checksum =
                            "41aab990705bab9497fe9ee410aa6d43e04de2054c765015ebe84ef07c2f3704",
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
                            "15b8e90ca80dc23125cd2925731035fdef20c749ba259df477d1dd103a06d621",
                    },
                Architecture.X64
                    => new SopsReleaseFileInfo()
                    {
                        ReleaseFileName = $"sops-{Version}.linux.amd64",
                        ExecutableFileName = "sops",
                        Sha256Checksum =
                            "d6bf07fb61972127c9e0d622523124c2d81caf9f7971fb123228961021811697",
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
