namespace DotnetSops.CommandLine.Services.Sops;
internal enum SopsEncryptionType
{
    AzureKeyVault,
    AwsKms,
    GcpKms,
    HashicorpVault,
    Age,
    Pgp
}
