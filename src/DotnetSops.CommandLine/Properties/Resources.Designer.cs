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
        ///   Looks up a localized string similar to Decrypt secrets into dotnet user secrets.
        /// </summary>
        internal static string DecryptCommandDescription {
            get {
                return ResourceManager.GetString("DecryptCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File of the encrypted secrets.
        /// </summary>
        internal static string DecryptCommandFileOptionDescription {
            get {
                return ResourceManager.GetString("DecryptCommandFileOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to project. Defaults to searching the current directory.
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
        ///   Looks up a localized string similar to &apos;{0}&apos; successfully decrypted to user secret with id &apos;{1}&apos;..
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
        ///   Looks up a localized string similar to Encrypt existing dotnet user secrets.
        /// </summary>
        internal static string EncryptCommandDescription {
            get {
                return ResourceManager.GetString("EncryptCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File of the encrypted secrets.
        /// </summary>
        internal static string EncryptCommandFileOptionDescription {
            get {
                return ResourceManager.GetString("EncryptCommandFileOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to project. Defaults to searching the current directory.
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
        ///   Looks up a localized string similar to User secret with id &apos;{0}&apos; successfully encrypted to &apos;{1}&apos;..
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
        ///   Looks up a localized string similar to What is public key of age?.
        /// </summary>
        internal static string InitializeCommandAgePublicKeyQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAgePublicKeyQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is object name of the key?.
        /// </summary>
        internal static string InitializeCommandAzureKeyVaultKeyQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyVaultKeyQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is the name of the key vault?.
        /// </summary>
        internal static string InitializeCommandAzureKeyVaultNameQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyVaultNameQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is object version of the key?.
        /// </summary>
        internal static string InitializeCommandAzureKeyVaultVersionQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandAzureKeyVaultVersionQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure to overwrite existing [yellow].sops.yaml[/]?.
        /// </summary>
        internal static string InitializeCommandConfigAlreadyExistQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandConfigAlreadyExistQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create .sops.yaml configuration file..
        /// </summary>
        internal static string InitializeCommandDescription {
            get {
                return ResourceManager.GetString("InitializeCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Which encryption whould you like to use?.
        /// </summary>
        internal static string InitializeCommandEncryptionQuestion {
            get {
                return ResourceManager.GetString("InitializeCommandEncryptionQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to What is public key of PGP?.
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
        ///   Looks up a localized string similar to You can now encrypt your dotnet user secrets by running:
        ///  [yellow]dotnet sops encrypt[/].
        /// </summary>
        internal static string InitializeCommandSuccessSuggestion {
            get {
                return ResourceManager.GetString("InitializeCommandSuccessSuggestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operating system is not supported..
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
        ///   Looks up a localized string similar to Ensure this property is set in the project or use the [yellow]&apos;--id&apos;[/] command line option.
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
        ///   Looks up a localized string similar to Store and share encrypted secrets, created using user-secrets tool.
        ///Secrets are encrypted and decrypted using SOPS.
        ///Encrypted secrets can shared with other team members than can decrypt it, if they have access.
        ///
        ///Encryption types is configured using .sops.yaml file. Run &quot;dotnet sops init&quot; for help wizard to create .sops.yaml.
        ///
        ///Warning: When secrets are decrypted they are stored in plain, unencrypted text, that can be loaded by user-secrets tool.
        ///Recomendation: Only store development secrets that canno [rest of string was truncated]&quot;;.
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
        ///   Looks up a localized string similar to SHA512 of SOPS executable did not match.
        ///
        ///Expected: {0}
        ///Actual:   {1}.
        /// </summary>
        internal static string SopsDownloadSha512Failed {
            get {
                return ResourceManager.GetString("SopsDownloadSha512Failed", resourceCulture);
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
        ///   Looks up a localized string similar to SOPS failed with error:
        ///
        ///{0}.
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
