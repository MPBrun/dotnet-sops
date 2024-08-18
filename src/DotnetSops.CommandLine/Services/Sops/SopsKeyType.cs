namespace DotnetSops.CommandLine.Services.Sops;

internal enum SopsKeyType
{
    AzureKeyVault,
    AwsKms,
    GcpKms,
    HashicorpVault,
    Age,
    Pgp,
}
