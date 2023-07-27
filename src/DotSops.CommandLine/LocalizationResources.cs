using System.Globalization;
using Spectre.Console;

namespace DotSops.CommandLine;
internal static class LocalizationResources
{
    public static string FileDoesNotExist(string filePath) => GetResourceString(Properties.Resources.FileDoesNotExist, filePath.EscapeMarkup());

    public static string UserSecretsFileDoesNotExist(string filePath) => GetResourceString(Properties.Resources.UserSecretsFileDoesNotExist, filePath.EscapeMarkup());

    public static string SopsDownloadHttpFailed(int statusCode, Uri? url) => GetResourceString(Properties.Resources.SopsDownloadHttpFailed, statusCode, url?.ToString()?.EscapeMarkup());

    public static string SopsDownloadSha512Failed(string expected, string actual) => GetResourceString(Properties.Resources.SopsDownloadSha512Failed, expected.EscapeMarkup(), actual.EscapeMarkup());

    public static string SopsRunFailedWithError(string output) => GetResourceString(Properties.Resources.SopsRunFailedWithError, output.EscapeMarkup());

    private static string GetResourceString(string resourceString, params object?[] formatArguments)
    {
        return string.Format(CultureInfo.InvariantCulture, resourceString, formatArguments);
    }
}
