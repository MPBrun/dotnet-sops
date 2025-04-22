using DotnetSops.CommandLine.Services.ProjectInfo;
using DotnetSops.CommandLine.Tests.Fixtures;

namespace DotnetSops.CommandLine.Tests.Services.ProjectInfo;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class ProjectInfoServiceTests : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _uniqueCurrentDirectoryFixture = new();

    protected virtual void Dispose(bool disposing)
    {
        _uniqueCurrentDirectoryFixture.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task FindUserSecretId_ProjectWithUserSecretsId_ReturnUserSecretId()
    {
        // Arrange
        var projectInfoService = new ProjectInfoService();

        var file = new FileInfo("Project.csproj");

        await File.WriteAllTextAsync(
            file.FullName,
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <UserSecretsId>1234</UserSecretsId>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """
        );

        // Act
        var userSecretId = projectInfoService.FindUserSecretId(file);

        // Assert
        Assert.Equal("1234", userSecretId);
    }

    [Fact]
    public async Task FindUserSecretId_NoFileSpecifiedWithSingleProject_FindSingleProject()
    {
        // Arrange
        var projectInfoService = new ProjectInfoService();

        await File.WriteAllTextAsync(
            "Project.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <UserSecretsId>1234</UserSecretsId>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """
        );

        // Act
        var userSecretId = projectInfoService.FindUserSecretId(null);

        // Assert
        Assert.Equal("1234", userSecretId);
    }

    [Fact]
    public async Task FindUserSecretId_NoFileSpecifiedNoProject_ThrowsProjectInfoSearchException()
    {
        // Arrange
        var projectInfoService = new ProjectInfoService();

        await File.WriteAllTextAsync(
            "Project.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <UserSecretsId>1234</UserSecretsId>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """
        );

        await File.WriteAllTextAsync(
            "Project.vbproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <UserSecretsId>1234</UserSecretsId>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """
        );

        // Act / Assert
        var exception = Assert.Throws<ProjectInfoSearchException>(() =>
            projectInfoService.FindUserSecretId(null)
        );
        Assert.Equal(
            $"Multiple MSBuild project files found in '{Directory.GetCurrentDirectory()}'.",
            exception.Message
        );
        Assert.Equal(
            "Specify which project to use with the [yellow]'--project'[/] option.",
            exception.Suggestion
        );
    }

    [Fact]
    public async Task FindUserSecretId_MultipleProject_IgnoresXProj()
    {
        // Arrange
        var projectInfoService = new ProjectInfoService();

        await File.WriteAllTextAsync(
            "Project.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <UserSecretsId>1234</UserSecretsId>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """
        );

        await File.WriteAllTextAsync(
            "Project.xproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
              </PropertyGroup>

            </Project>
            """
        );

        // Act
        var userSecretId = projectInfoService.FindUserSecretId(null);

        // Assert
        Assert.Equal("1234", userSecretId);
    }

    [Fact]
    public void FindUserSecretId_NoFileSpecifiedMulitpleProject_ThrowsProjectInfoSearchException()
    {
        // Arrange
        var projectInfoService = new ProjectInfoService();

        // Act / Assert
        var exception = Assert.Throws<ProjectInfoSearchException>(() =>
            projectInfoService.FindUserSecretId(null)
        );
        Assert.Equal(
            $"Could not find a MSBuild project file in '{Directory.GetCurrentDirectory()}'.",
            exception.Message
        );
        Assert.Equal(
            "Specify which project to use with the [yellow]'--project'[/] option or use the [yellow]'--id'[/] option.",
            exception.Suggestion
        );
    }

    [Fact]
    public async Task FindUserSecretId_ProjectNoUserSecretsId_ThrowsProjectInfoSearchException()
    {
        // Arrange
        var projectInfoService = new ProjectInfoService();

        var file = new FileInfo("Project.csproj");

        await File.WriteAllTextAsync(
            file.FullName,
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """
        );

        // Act / Assert
        var exception = Assert.Throws<ProjectInfoSearchException>(() =>
            projectInfoService.FindUserSecretId(file)
        );
        Assert.Equal(
            $"Could not find the global property 'UserSecretsId' in MSBuild project '{file.FullName}'.",
            exception.Message
        );
        Assert.Equal(
            """
            Ensure this property is set in the project or use the [yellow]'--id'[/] command-line option.

            The 'UserSecretsId' property can be created by running this command:
              [yellow]dotnet user-secrets init[/]
            """,
            exception.Suggestion
        );
    }

    [Fact]
    public async Task FindUserSecretId_InvalidProjectFile_ThrowsProjectInfoSearchException()
    {
        // Arrange
        var projectInfoService = new ProjectInfoService();

        var file = new FileInfo("Project.csproj");

        await File.WriteAllTextAsync(
            file.FullName,
            """
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramew
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>

            </Project>
            """
        );

        // Act / Assert
        var exception = Assert.Throws<ProjectInfoSearchException>(() =>
            projectInfoService.FindUserSecretId(file)
        );
        Assert.Equal($"Could not load the MSBuild project '{file.FullName}'.", exception.Message);
        Assert.Null(exception.Suggestion);
    }

    [Fact]
    public void FindUserSecretId_ProjectFileNotExist_ThrowsProjectInfoSearchException()
    {
        // Arrange
        var projectInfoService = new ProjectInfoService();

        var file = new FileInfo("Project.csproj");

        // Act / Assert
        var exception = Assert.Throws<ProjectInfoSearchException>(() =>
            projectInfoService.FindUserSecretId(file)
        );
        Assert.Equal($"File '{file.FullName}' does not exist.", exception.Message);
        Assert.Null(exception.Suggestion);
    }
}
