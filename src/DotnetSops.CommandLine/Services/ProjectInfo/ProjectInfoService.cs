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
            var projectFiles = Directory.EnumerateFiles(currentDirectory, "*.*proj", SearchOption.TopDirectoryOnly)
                .Where(f => !".xproj".Equals(Path.GetExtension(f), StringComparison.OrdinalIgnoreCase))
                .Select(file => new FileInfo(file))
                .ToList();
            if (projectFiles.Count > 1)
            {
                throw new ProjectInfoSearchException($"Multiple MSBuild project files found in '{currentDirectory}'.")
                {
                    Suggestion = "Specify which project to use with the [yellow]'--project'[/] option."
                };
            }
            projectFile = projectFiles.Count == 0
                ? throw new ProjectInfoSearchException($"Could not find a MSBuild project file in '{currentDirectory}'.")
                {
                    Suggestion = "Specify which project to use with the [yellow]'--project'[/] option or use the [yellow]'--id'[/] option."
                }
                : projectFiles[0];
        }

        if (!projectFile.Exists)
        {
            throw new ProjectInfoSearchException(LocalizationResources.FileDoesNotExist(projectFile.FullName));
        }
        try
        {
            var projectDocument = XDocument.Load(projectFile.FullName, LoadOptions.PreserveWhitespace);
            var userSecretId = projectDocument.XPathSelectElements("//UserSecretsId").FirstOrDefault()?.Value;

            return string.IsNullOrWhiteSpace(userSecretId)
                ? throw new ProjectInfoSearchException($"Could not find the global property 'UserSecretsId' in MSBuild project '{projectFile.FullName}'")
                {
                    Suggestion = """
                            Ensure this property is set in the project or use the [yellow]'--id'[/] command line option.
                            
                            The 'UserSecretsId' property can be created by running this command:
                              [yellow]dotnet user-secrets init[/]
                            """
                }
                : userSecretId;
        }
        catch (ProjectInfoSearchException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ProjectInfoSearchException($"Could not load the MSBuild project '{projectFile.FullName}'.");
        }
    }
}
