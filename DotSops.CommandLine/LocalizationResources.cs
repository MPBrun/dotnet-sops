using System.Globalization;

namespace DotSops.CommandLine;
internal static class LocalizationResources
{
    public static string FileDoesNotExist(string filePath) => GetResourceString(Properties.Resources.FileDoesNotExist, filePath);

    public static string UserSecretsFileDoesNotExist(string filePath) => GetResourceString(Properties.Resources.UserSecretsFileDoesNotExist, filePath);

    public static string SopsDownloadHttpFailed(int statusCode, Uri? url) => GetResourceString(Properties.Resources.SopsDownloadHttpFailed, statusCode, url);

    public static string SopsDownloadSha512Failed(string expected, string actual) => GetResourceString(Properties.Resources.SopsDownloadSha512Failed, expected, actual);

    public static string SopsRunFailed(string output) => GetResourceString(Properties.Resources.SopsRunFailed, output);

    private static string GetResourceString(string resourceString, params object?[] formatArguments)
    {
        return string.Format(CultureInfo.InvariantCulture, resourceString, formatArguments);
    }
}
