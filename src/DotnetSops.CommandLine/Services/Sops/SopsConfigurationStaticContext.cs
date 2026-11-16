using YamlDotNet.Serialization;

namespace DotnetSops.CommandLine.Services.Sops;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Required by YamlDotNet source generator"
)]
[YamlStaticContext]
[YamlSerializable(typeof(SopsConfiguration))]
[YamlSerializable(typeof(SopsCreationRule))]
[YamlSerializable(typeof(SopsKeyGroup))]
[YamlSerializable(typeof(SopsAzureKeyVaultKeyGroup))]
[YamlSerializable(typeof(SopsKmsKeyGroup))]
[YamlSerializable(typeof(SopsGcpKmsKeyGroup))]
public partial class SopsConfigurationStaticContext : StaticContext { }
