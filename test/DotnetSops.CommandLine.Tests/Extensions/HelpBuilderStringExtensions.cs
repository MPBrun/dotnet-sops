namespace DotnetSops.CommandLine.Tests.Extensions;
internal static class HelpBuilderStringExtensions
{
    /// <summary>
    /// Removes wrap newlines, that are added by HelpBuilder when test are runnnig from PowerShell with a Width, lines are being broken up by HelpBuilder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveHelpWrapNewLines(this string value)
    {
        return value
            .Replace($"  {Environment.NewLine}  ", $"###{Environment.NewLine}###", StringComparison.InvariantCulture) // Add marker for newlines to keep
            .Replace($" {Environment.NewLine}  ", " ", StringComparison.InvariantCulture) // Replace newlines starting with a single space
            .Replace($"###{Environment.NewLine}###", $"  {Environment.NewLine}  ", StringComparison.InvariantCulture); // Replace marker again
    }
}
