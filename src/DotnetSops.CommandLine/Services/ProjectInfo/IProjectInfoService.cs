namespace DotnetSops.CommandLine.Services.ProjectInfo;

internal interface IProjectInfoService
{
    /// <summary>
    /// Find UserSecretId in project file specified.
    /// </summary>
    /// <param name="projectFile">Project file to search. If null search for project file in the current directory.</param>
    /// <returns></returns>
    /// <exception cref="ProjectInfoSearchException"></exception>
    string FindUserSecretId(FileInfo? projectFile);
}
