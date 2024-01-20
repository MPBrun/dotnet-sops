using System.Xml.Linq;
using System.Xml.XPath;

namespace DotnetSops.CommandLine.Services.ProjectInfo;

internal class ProjectInfoService : IProjectInfoService
{
    public string FindUserSecretId(FileInfo? projectFile)
    {
        if (projectFile == null)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var projectFiles = Directory
                .EnumerateFiles(currentDirectory, "*.*proj", SearchOption.TopDirectoryOnly)
                .Where(f =>
                    !".xproj".Equals(Path.GetExtension(f), StringComparison.OrdinalIgnoreCase)
                )
                .Select(file => new FileInfo(file))
                .ToList();
            if (projectFiles.Count > 1)
            {
                throw new ProjectInfoSearchException(
                    LocalizationResources.ProjectInfoServiceMultipleFoundError(currentDirectory)
                )
                {
                    Suggestion = Properties.Resources.ProjectInfoServiceMultipleFoundSuggestion
                };
            }
            projectFile =
                projectFiles.Count == 0
                    ? throw new ProjectInfoSearchException(
                        LocalizationResources.ProjectInfoServiceNotFoundError(currentDirectory)
                    )
                    {
                        Suggestion = Properties.Resources.ProjectInfoServiceNotFoundSuggestion
                    }
                    : projectFiles[0];
        }

        if (!projectFile.Exists)
        {
            throw new ProjectInfoSearchException(
                LocalizationResources.FileDoesNotExist(projectFile.FullName)
            );
        }
        try
        {
            var projectDocument = XDocument.Load(
                projectFile.FullName,
                LoadOptions.PreserveWhitespace
            );
            var userSecretId = projectDocument
                .XPathSelectElements("//UserSecretsId")
                .FirstOrDefault()
                ?.Value;

            return string.IsNullOrWhiteSpace(userSecretId)
                ? throw new ProjectInfoSearchException(
                    LocalizationResources.ProjectInfoServiceUserSecretIdNotFoundError(
                        projectFile.FullName
                    )
                )
                {
                    Suggestion = Properties
                        .Resources
                        .ProjectInfoServiceUserSecretIdNotFoundSuggestion
                }
                : userSecretId;
        }
        catch (ProjectInfoSearchException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ProjectInfoSearchException(
                LocalizationResources.ProjectInfoServiceNotLoadableError(projectFile.FullName)
            );
        }
    }
}
