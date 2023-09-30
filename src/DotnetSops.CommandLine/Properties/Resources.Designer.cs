﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DotnetSops.CommandLine.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DotnetSops.CommandLine.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Decrypt secrets into .NET User Secrets.
        /// </summary>
        internal static string DecryptCommandDescription {
            get {
                return ResourceManager.GetString("DecryptCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encrypted secrets file.
        /// </summary>
        internal static string DecryptCommandFileOptionDescription {
            get {
                return ResourceManager.GetString("DecryptCommandFileOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to the project. Defaults to searching the current directory..
        /// </summary>
        internal static string DecryptCommandProjectOptionDescription {
            get {
                return ResourceManager.GetString("DecryptCommandProjectOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user secret ID to use..
        /// </summary>
        internal static string DecryptCommandSecretsIdOptionDescription {
            get {
                return ResourceManager.GetString("DecryptCommandSecretsIdOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; successfully decrypted to user secret with ID &apos;{1}&apos;..
        /// </summary>
        internal static string DecryptCommandSuccess {
            get {
                return ResourceManager.GetString("DecryptCommandSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Download SOPS from https://github.com/getsops/sops.
        /// </summary>
        internal static string DownloadSopsCommandDescription {
            get {
                return ResourceManager.GetString("DownloadSopsCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading [yellow]SOPS[/] from [link]https://github.com/getsops/sops[/].
        /// </summary>
        internal static string DownloadSopsCommandInformation {
            get {
                return ResourceManager.GetString("DownloadSopsCommandInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SOPS has been successfully downloaded..
        /// </summary>
        internal static string DownloadSopsCommandSuccess {
            get {
                return ResourceManager.GetString("DownloadSopsCommandSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading SOPS.
        /// </summary>
        internal static string DownloadSopsLoader {
            get {
                return ResourceManager.GetString("DownloadSopsLoader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encrypt existing .NET User Secrets.
        /// </summary>
        internal static string EncryptCommandDescription {
            get {
                return ResourceManager.GetString("EncryptCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encrypted secrets file..
        /// </summary>
        internal static string EncryptCommandFileOptionDescription {
            get {
                return ResourceManager.GetString("EncryptCommandFileOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to the project. Defaults to searching the current directory..
        /// </summary>
        internal static string EncryptCommandProjectOptionDescription {
            get {
                return ResourceManager.GetString("EncryptCommandProjectOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user secret ID to use..
        /// </summary>
        internal static string EncryptCommandSecretsIdOptionDescription {
            get {
                return ResourceManager.GetString("EncryptCommandSecretsIdOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User secret with ID &apos;{0}&apos; successfully encrypted to &apos;{1}&apos;..
        /// </summary>
        internal static string EncryptCommandSuccess {
            get {
                return ResourceManager.GetString("EncryptCommandSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string FileDoesNotExist {
            get {
                return ResourceManager.GetString("FileDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the public key of age?.
        /// </summary>
        internal static string InitializeCommandAgePublicKeyQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAgePublicKeyQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An ARN has following format: arn:partition:service:region:account-id:resource-type/resource-id.
        /// </summary>
        internal static string InitializeCommandAwsKmsFormat {
            get {
                return ResourceManager.GetString("InitializeCommandAwsKmsFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the ARN of the key?.
        /// </summary>
        internal static string InitializeCommandAwsKmsQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAwsKmsQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the key identifier of the key?.
        /// </summary>
        internal static string InitializeCommandAzureKeyIdentifierQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyIdentifierQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An Azure Vault key is defined by 3 parts of the key identifier: https://{vault-name}.vault.azure.net/keys/{object-name}/{object-version}.
        /// </summary>
        internal static string InitializeCommandAzureKeyVaultFormat {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyVaultFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A key identifier has following format: https://{vault-name}.vault.azure.net/keys/{object-name}/{object-version}.
        /// </summary>
        internal static string InitializeCommandAzureKeyVaultIdentifierFormat {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyVaultIdentifierFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the object name of the key?.
        /// </summary>
        internal static string InitializeCommandAzureKeyVaultKeyQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyVaultKeyQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the name of the Key Vault?.
        /// </summary>
        internal static string InitializeCommandAzureKeyVaultNameQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyVaultNameQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the object version of the key?.
        /// </summary>
        internal static string InitializeCommandAzureKeyVaultVersionQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyVaultVersionQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to overwrite the existing [yellow].sops.yaml[/]?.
        /// </summary>
        internal static string InitializeCommandConfigAlreadyExistQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandConfigAlreadyExistQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create a .sops.yaml configuration file..
        /// </summary>
        internal static string InitializeCommandDescription {
            get {
                return ResourceManager.GetString("InitializeCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The resource ID has the following format: projects/PROJECT_ID/locations/LOCATION/keyRings/KEY_RING/cryptoKeys/KEY_NAME.
        /// </summary>
        internal static string InitializeCommandGcpKmsFormat {
            get {
                return ResourceManager.GetString("InitializeCommandGcpKmsFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the resource ID of the key?.
        /// </summary>
        internal static string InitializeCommandGcpKmsQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandGcpKmsQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the URI of the vault key?.
        /// </summary>
        internal static string InitializeCommandHashicorpVaultQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandHashicorpVaultQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [yellow]Keys:[/]
        ///  SOPS supports different keys. You can refer to their respective documentation on how to create a key that supports encryption and decryption.
        ///
        ///[yellow]Documentation:[/]
        ///  [yellow]Azure Key Vault[/] - https://learn.microsoft.com/en-us/azure/key-vault
        ///  [yellow]AWS KMS[/] - https://aws.amazon.com/kms
        ///  [yellow]GCP KMS[/] - https://cloud.google.com/security-key-management
        ///  [yellow]Hashicorp Vault[/] - https://www.vaultproject.io
        ///  [yellow]age[/] - https://github.com/FiloSottile/age        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string InitializeCommandKeyInformation {
            get {
                return ResourceManager.GetString("InitializeCommandKeyInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Which key type would you like to use?.
        /// </summary>
        internal static string InitializeCommandKeyTypeQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandKeyTypeQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Add more key groups?.
        /// </summary>
        internal static string InitializeCommandMoreKeyGroupsQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandMoreKeyGroupsQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Add more keys?.
        /// </summary>
        internal static string InitializeCommandMoreKeysQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandMoreKeysQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Add more keys to the group?.
        /// </summary>
        internal static string InitializeCommandMoreKeysToKeyGroupQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandMoreKeysToKeyGroupQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the public key of PGP?.
        /// </summary>
        internal static string InitializeCommandPgpPublicKeyQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandPgpPublicKeyQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [green]Generated .sops.yaml with the following content:[/].
        /// </summary>
        internal static string InitializeCommandSuccessGenerated {
            get {
                return ResourceManager.GetString("InitializeCommandSuccessGenerated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You can now encrypt your .NET User Secrets by running:
        ///  [yellow]dotnet sops encrypt[/].
        /// </summary>
        internal static string InitializeCommandSuccessSuggestion {
            get {
                return ResourceManager.GetString("InitializeCommandSuccessSuggestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use key groups?.
        /// </summary>
        internal static string InitializeCommandUseKeyGroupsQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandUseKeyGroupsQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to age.
        /// </summary>
        internal static string KeyTypeAge {
            get {
                return ResourceManager.GetString("KeyTypeAge", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AWS KMS.
        /// </summary>
        internal static string KeyTypeAwsKms {
            get {
                return ResourceManager.GetString("KeyTypeAwsKms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Azure Key Vault.
        /// </summary>
        internal static string KeyTypeAzureKeyVault {
            get {
                return ResourceManager.GetString("KeyTypeAzureKeyVault", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GCP KMS.
        /// </summary>
        internal static string KeyTypeGcpKms {
            get {
                return ResourceManager.GetString("KeyTypeGcpKms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hashicorp Vault.
        /// </summary>
        internal static string KeyTypeHashicorpVault {
            get {
                return ResourceManager.GetString("KeyTypeHashicorpVault", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PGP.
        /// </summary>
        internal static string KeyTypePgp {
            get {
                return ResourceManager.GetString("KeyTypePgp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operating system is not supported..
        /// </summary>
        internal static string OperatingSystemNotSupported {
            get {
                return ResourceManager.GetString("OperatingSystemNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple MSBuild project files found in &apos;{0}&apos;..
        /// </summary>
        internal static string ProjectInfoServiceMultipleFoundError {
            get {
                return ResourceManager.GetString("ProjectInfoServiceMultipleFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specify which project to use with the [yellow]&apos;--project&apos;[/] option..
        /// </summary>
        internal static string ProjectInfoServiceMultipleFoundSuggestion {
            get {
                return ResourceManager.GetString("ProjectInfoServiceMultipleFoundSuggestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a MSBuild project file in &apos;{0}&apos;..
        /// </summary>
        internal static string ProjectInfoServiceNotFoundError {
            get {
                return ResourceManager.GetString("ProjectInfoServiceNotFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specify which project to use with the [yellow]&apos;--project&apos;[/] option or use the [yellow]&apos;--id&apos;[/] option..
        /// </summary>
        internal static string ProjectInfoServiceNotFoundSuggestion {
            get {
                return ResourceManager.GetString("ProjectInfoServiceNotFoundSuggestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not load the MSBuild project &apos;{0}&apos;..
        /// </summary>
        internal static string ProjectInfoServiceNotLoadableError {
            get {
                return ResourceManager.GetString("ProjectInfoServiceNotLoadableError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find the global property &apos;UserSecretsId&apos; in MSBuild project &apos;{0}&apos;..
        /// </summary>
        internal static string ProjectInfoServiceUserSecretIdNotFoundError {
            get {
                return ResourceManager.GetString("ProjectInfoServiceUserSecretIdNotFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ensure this property is set in the project or use the [yellow]&apos;--id&apos;[/] command-line option.
        ///
        ///The &apos;UserSecretsId&apos; property can be created by running this command:
        ///  [yellow]dotnet user-secrets init[/].
        /// </summary>
        internal static string ProjectInfoServiceUserSecretIdNotFoundSuggestion {
            get {
                return ResourceManager.GetString("ProjectInfoServiceUserSecretIdNotFoundSuggestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encrypt and share secrets with the user-secrets tool, utilizing SOPS for encryption and decryption. Encrypted secrets can be shared with other team members who can decrypt them if they have access.
        ///
        ///Key types are configured using the .sops.yaml file. Run &apos;dotnet sops init&apos; for a helpful wizard to create the .sops.yaml..
        /// </summary>
        internal static string RootCommandDescription {
            get {
                return ResourceManager.GetString("RootCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enable verbose logging output.
        /// </summary>
        internal static string RootDotnetSopsCommandVerboseOptionDescription {
            get {
                return ResourceManager.GetString("RootDotnetSopsCommandVerboseOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arguments passed to the &apos;dotnet run&apos; command..
        /// </summary>
        internal static string RunCommandArgumentsDescription {
            get {
                return ResourceManager.GetString("RunCommandArgumentsDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute &apos;dotnet run&apos; with decrypted secrets inserted into the environment.
        /// </summary>
        internal static string RunCommandDescription {
            get {
                return ResourceManager.GetString("RunCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encrypted secrets file.
        /// </summary>
        internal static string RunCommandFileOptionDescription {
            get {
                return ResourceManager.GetString("RunCommandFileOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to download SOPS.
        ///
        ///HTTP status code: {0}
        ///URL: {1}.
        /// </summary>
        internal static string SopsDownloadHttpFailed {
            get {
                return ResourceManager.GetString("SopsDownloadHttpFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SHA256 of SOPS executable did not match.
        ///
        ///Expected: {0}
        ///Actual:   {1}.
        /// </summary>
        internal static string SopsDownloadSha256Failed {
            get {
                return ResourceManager.GetString("SopsDownloadSha256Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SOPS executable could not be found on the PATH..
        /// </summary>
        internal static string SopsIsMissing {
            get {
                return ResourceManager.GetString("SopsIsMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You can download it by executing the following command:
        ///  [yellow]dotnet sops download-sops[/].
        /// </summary>
        internal static string SopsIsMissingSuggestion {
            get {
                return ResourceManager.GetString("SopsIsMissingSuggestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to run sops..
        /// </summary>
        internal static string SopsRunFailed {
            get {
                return ResourceManager.GetString("SopsRunFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Executing SOPS failed..
        /// </summary>
        internal static string SopsRunFailedWithError {
            get {
                return ResourceManager.GetString("SopsRunFailedWithError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User secrets file &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string UserSecretsFileDoesNotExist {
            get {
                return ResourceManager.GetString("UserSecretsFileDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have no secrets created. You can add secrets by running this command:
        ///  [yellow]dotnet user-secrets set [[name]] [[value]][/].
        /// </summary>
        internal static string UserSecretsFileDoesNotExistSuggestion {
            get {
                return ResourceManager.GetString("UserSecretsFileDoesNotExistSuggestion", resourceCulture);
            }
        }
    }
}
