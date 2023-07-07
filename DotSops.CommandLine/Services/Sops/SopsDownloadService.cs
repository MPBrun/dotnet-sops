using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DotSops.CommandLine.Services.PlatformInformation;

namespace DotSops.CommandLine.Services.Sops;
internal class SopsDownloadService : ISopsDownloadService
{
    private const string Version = "3.7.3";
    private readonly IPlatformInformationService _platformInformation;
    private readonly HttpClient _httpClient;

    public SopsDownloadService(IPlatformInformationService platformInformation, HttpClient httpClient)
    {
        _platformInformation = platformInformation;
        _httpClient = httpClient;
    }

    public async Task DownloadAsync(CancellationToken cancellationToken = default)
    {
        var release = PlatformSopsReleaseFile();

        // Download sops
        var platformFile = release.ReleaseFileName;
        var url = new Uri($"https://github.com/mozilla/sops/releases/download/v{Version}/{platformFile}");
        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new DotSopsException(LocalizationResources.SopsDownloadHttpFailed((int)response.StatusCode, url));
        }

        // Get content
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        // Verify content hash
        var hash = BitConverter.ToString(SHA512.HashData(content)).Replace("-", "", StringComparison.OrdinalIgnoreCase); // Same as cli "certutil -hashfile sops.exe SHA512"
        if (!hash.Equals(release.Sha512Checksum, StringComparison.OrdinalIgnoreCase))
        {
            throw new DotSopsException(LocalizationResources.SopsDownloadSha512Failed(release.Sha512Checksum.ToUpperInvariant(), hash.ToUpperInvariant()));
        }

        // Save content to disk
        var destinationFileName = release.ExecutableFileName;
        await File.WriteAllBytesAsync(destinationFileName, content, cancellationToken);
    }
    private SopsReleaseFileInfo PlatformSopsReleaseFile()
    {
        if (_platformInformation.IsWindows())
        {
            return new SopsReleaseFileInfo()
            {
                ReleaseFileName = $"sops-v{Version}.exe",
                ExecutableFileName = "sops.exe",
                Sha512Checksum = "0b6f768d388a9316838efc58bebe16f0598b84f45643249996bdc6d2e237b5b01c65069809cfc153a3e97402691f4c1879d3ea4920b7de03b38eefd17c717c2d",
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
                    Sha512Checksum = "1307f5b8ad79903a93ed31055d656a8f286ce88e924bac940eba0247513a6b0764b94f39c8c68fa6b760115c0e3356ae25700da6278afae6a53f5256cf13c07f",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.darwin.amd64",
                    ExecutableFileName = "sops",
                    Sha512Checksum = "b323b9c49c1635350026e37cfb0d67130312ea5b9e152c164697f2ffcfa3fe7be20a2bb4373ef7829117964011bfb5598026e27453df5d46e2961ee92b6f0dd0",
                },
                Architecture.X86 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.darwin",
                    ExecutableFileName = "sops",
                    Sha512Checksum = "b323b9c49c1635350026e37cfb0d67130312ea5b9e152c164697f2ffcfa3fe7be20a2bb4373ef7829117964011bfb5598026e27453df5d46e2961ee92b6f0dd0",
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
                    Sha512Checksum = "56f8e71871fbc202504b42fea6eee93acc6c6a8194db3c335f9ddc3e90a4d8106ad4ad95b1e4d24f8723596302d6fff154b973ecb4a5fd6cd9f6aead0a96fe1d",
                },
                Architecture.X64 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.linux.amd64",
                    ExecutableFileName = "sops",
                    Sha512Checksum = "8883090802db42432dffb34a31c614a3a646c4a38bb43ea71bdbfa01cc22fac4b6b390d9b82c14a50f14770fd0089f0dba2895083e224007989c7a5672588bc7",
                },
                Architecture.X86 => new SopsReleaseFileInfo()
                {
                    ReleaseFileName = $"sops-v{Version}.linux",
                    ExecutableFileName = "sops",
                    Sha512Checksum = "8883090802db42432dffb34a31c614a3a646c4a38bb43ea71bdbfa01cc22fac4b6b390d9b82c14a50f14770fd0089f0dba2895083e224007989c7a5672588bc7",
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
        throw new NotSupportedException(Properties.Resources.OperatingSystemNotSupported);
    }
}
