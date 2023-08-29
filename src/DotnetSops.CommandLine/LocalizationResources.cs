using System.Globalization;

namespace DotnetSops.CommandLine;
internal static class LocalizationResources
{
    /// <inheritdoc cref="Properties.Resources.FileDoesNotExist"/>
    public static string FileDoesNotExist(string filePath) => GetResourceString(Properties.Resources.FileDoesNotExist, filePath);

    /// <inheritdoc cref="Properties.Resources.UserSecretsFileDoesNotExist"/>
    public static string UserSecretsFileDoesNotExist(string filePath) => GetResourceString(Properties.Resources.UserSecretsFileDoesNotExist, filePath);

    /// <inheritdoc cref="Properties.Resources.SopsDownloadHttpFailed"/>
    public static string SopsDownloadHttpFailed(int statusCode, Uri? url) => GetResourceString(Properties.Resources.SopsDownloadHttpFailed, statusCode, url?.ToString());

    /// <inheritdoc cref="Properties.Resources.SopsDownloadSha512Failed"/>
    public static string SopsDownloadSha512Failed(string expected, string actual) => GetResourceString(Properties.Resources.SopsDownloadSha512Failed, expected, actual);

    /// <inheritdoc cref="Properties.Resources.DecryptCommandSuccess"/>
    public static string DecryptCommandSuccess(string filePath, string userSecretId) => GetResourceString(Properties.Resources.DecryptCommandSuccess, filePath, userSecretId);

    /// <inheritdoc cref="Properties.Resources.EncryptCommandSuccess"/>
    public static string EncryptCommandSuccess(string userSecretId, string filePath) => GetResourceString(Properties.Resources.EncryptCommandSuccess, userSecretId, filePath);

    /// <inheritdoc cref="Properties.Resources.ProjectInfoServiceMultipleFoundError"/>
    public static string ProjectInfoServiceMultipleFoundError(string currentDirectory) => GetResourceString(Properties.Resources.ProjectInfoServiceMultipleFoundError, currentDirectory);

    /// <inheritdoc cref="Properties.Resources.ProjectInfoServiceNotFoundError"/>
    public static string ProjectInfoServiceNotFoundError(string currentDirectory) => GetResourceString(Properties.Resources.ProjectInfoServiceNotFoundError, currentDirectory);

    /// <inheritdoc cref="Properties.Resources.ProjectInfoServiceUserSecretIdNotFoundError"/>
    public static string ProjectInfoServiceUserSecretIdNotFoundError(string projectFilePath) => GetResourceString(Properties.Resources.ProjectInfoServiceUserSecretIdNotFoundError, projectFilePath);

    /// <inheritdoc cref="Properties.Resources.ProjectInfoServiceNotLoadableError"/>
    public static string ProjectInfoServiceNotLoadableError(string projectFilePath) => GetResourceString(Properties.Resources.ProjectInfoServiceNotLoadableError, projectFilePath);

    private static string GetResourceString(string resourceString, params object?[] formatArguments)
    {
        return string.Format(CultureInfo.InvariantCulture, resourceString, formatArguments);
    }
}
