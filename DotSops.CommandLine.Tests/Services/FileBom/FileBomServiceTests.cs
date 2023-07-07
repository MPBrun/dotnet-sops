using System.Text;
using DotSops.CommandLine.Services.FileBom;

namespace DotSops.CommandLine.Tests.Services.FileBom;
public class FileBomServiceTests
{
    [Fact]
    public async Task RemoveBomFromFileAsync_WithBomInFile_RemovesBom()
    {
        // Arrange
        var service = new FileBomService();
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var filePath = new FileInfo(Path.Combine(dir.FullName, "file.json"));

        await File.AppendAllTextAsync(filePath.FullName, "content", new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        var creationTime = File.GetLastWriteTimeUtc(filePath.FullName);

        var fileContentBefore = await File.ReadAllBytesAsync(filePath.FullName);
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, fileContentBefore.Take(3).ToArray());
        Assert.Equal(10, fileContentBefore.Length);

        // Act
        await service.RemoveBomFromFileAsync(filePath);

        // Assert
        var fileContentAfter = await File.ReadAllBytesAsync(filePath.FullName);
        Assert.NotEqual(new byte[] { 0xEF, 0xBB, 0xBF }, fileContentAfter.Take(3).ToArray());
        Assert.Equal(7, fileContentAfter.Length);

        var updateTime = File.GetLastWriteTimeUtc(filePath.FullName);
        Assert.NotEqual(creationTime, updateTime);
    }

    [Fact]
    public async Task RemoveBomFromFileAsync_NoBomInFile_NotChanged()
    {
        // Arrange
        var service = new FileBomService();
        var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());
        var filePath = new FileInfo(Path.Combine(dir.FullName, "file.json"));

        await File.AppendAllTextAsync(filePath.FullName, "content", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        var creationTime = File.GetLastWriteTimeUtc(filePath.FullName);

        var fileContentBefore = await File.ReadAllBytesAsync(filePath.FullName);
        Assert.NotEqual(new byte[] { 0xEF, 0xBB, 0xBF }, fileContentBefore.Take(3).ToArray());
        Assert.Equal(7, fileContentBefore.Length);

        // Act
        await service.RemoveBomFromFileAsync(filePath);

        // Assert
        var fileContentAfter = await File.ReadAllBytesAsync(filePath.FullName);
        Assert.NotEqual(new byte[] { 0xEF, 0xBB, 0xBF }, fileContentAfter.Take(3).ToArray());
        Assert.Equal(7, fileContentAfter.Length);

        var updateTime = File.GetLastWriteTimeUtc(filePath.FullName);
        Assert.Equal(creationTime, updateTime);
    }
}
