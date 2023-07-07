using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Text.Json;
using DotSops.CommandLine.Commands;
using DotSops.CommandLine.Services.FileBom;
using DotSops.CommandLine.Services.Sops;
using DotSops.CommandLine.Services.UserSecrets;
using DotSops.CommandLine.Tests.Services.UserSecrets;

namespace DotSops.CommandLine.Tests.Commands;
public class EncryptCommandTests
{
    [Fact]
    public async Task EncryptCommand_ValidOptions_CreateFile()
    {
        // Arrange
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var sopsService = new SopsService(dir.FullName);
        var userSecretsService = new UserSecretsServiceStub(dir.FullName);
        var fileBomService = new FileBomService();

        var command = new EncryptCommand();
        var id = $"unittest-{Guid.NewGuid()}";

        // user secret
        var filePath = userSecretsService.GetSecretsPathFromSecretsId(id);
        await File.AppendAllTextAsync(filePath.FullName, JsonSerializer.Serialize(new object()), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); //dotnet user secrets save files with bom

        // Sops config
        var sopsConfigPath = Path.Combine(dir.FullName, ".sops.yaml");
        await File.WriteAllTextAsync(sopsConfigPath, """
            creation_rules:
                - path_regex: .*.json
                  age: 'age196za9tkwypwclcacrjea7jsggl3jwntpx3ms6yj5vc4unkz2d4sqvazcn8'
            """);

        var outputPath = Path.Combine(dir.FullName, "secrets.json");

        var parseResult = command.Parse($"--id {id} --file {outputPath}");
        using var invocationContext = new InvocationContext(parseResult);
        invocationContext.BindingContext.AddService(typeof(ISopsService), sp => sopsService);
        invocationContext.BindingContext.AddService(typeof(IUserSecretsService), sp => userSecretsService);
        invocationContext.BindingContext.AddService(typeof(IFileBomService), sp => fileBomService);

        // Act
        var exitCode = await command.Handler!.InvokeAsync(invocationContext);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.True(File.Exists(outputPath));
    }

    [Fact]
    public void EncryptCommand_Id_Required()
    {
        // Arrange
        var command = new EncryptCommand();

        // Act / Assert
        var option = command.Options.First(o => o.Name == "id");
        Assert.True(option.IsRequired);
    }

    [Fact]
    public void EncryptCommand_File_NotRequired()
    {
        // Arrange
        var command = new EncryptCommand();

        // Act / Assert
        var option = command.Options.First(o => o.Name == "file");
        Assert.False(option.IsRequired);
    }
}
