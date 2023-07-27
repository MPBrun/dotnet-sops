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
                throw new ProjectInfoSearchException($"""
                        Multiple MSBuild project files found in '{currentDirectory}'.
                        Specify which to use with the --project option
                        """);
            }
            projectFile = projectFiles.Count == 0
                ? throw new ProjectInfoSearchException($"""
                        Could not find a MSBuild project file in '{currentDirectory}'.
                        Specify which project to use with the --project option or use the '--id' option.
                        """)
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
                ? throw new ProjectInfoSearchException($"""
                        Could not find the global property 'UserSecretsId' in MSBuild project '{projectFile.FullName}'
                        Ensure this property is set in the project or use the '--id' command line option.
                        """)
                : userSecretId;
        }
        catch (ProjectInfoSearchException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ProjectInfoSearchException($"""
                        Could not load the MSBuild project '{projectFile.FullName}'.
                        """);
        }
    }
}
