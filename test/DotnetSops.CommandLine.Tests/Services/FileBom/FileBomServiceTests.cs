using System.Text;
using DotnetSops.CommandLine.Services.FileBom;
using DotnetSops.CommandLine.Tests.Fixtures;

namespace DotnetSops.CommandLine.Tests.Services.FileBom;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class FileBomServiceTests : IDisposable
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
    public async Task RemoveBomFromFileAsync_WithBomInFile_RemovesBom()
    {
        // Arrange
        var service = new FileBomService();
        var filePath = new FileInfo("file.json");

        await File.AppendAllTextAsync(filePath.FullName, "content", new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        var fileContentBefore = await File.ReadAllBytesAsync(filePath.FullName);
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, fileContentBefore.Take(3).ToArray());
        Assert.Equal(10, fileContentBefore.Length);

        // Act
        await service.RemoveBomFromFileAsync(filePath);

        // Assert
        var fileContentAfter = await File.ReadAllBytesAsync(filePath.FullName);
        Assert.NotEqual(new byte[] { 0xEF, 0xBB, 0xBF }, fileContentAfter.Take(3).ToArray());
        Assert.Equal(7, fileContentAfter.Length);
    }

    [Fact]
    public async Task RemoveBomFromFileAsync_NoBomInFile_NotChanged()
    {
        // Arrange
        var service = new FileBomService();
        var filePath = new FileInfo("file.json");

        await File.AppendAllTextAsync(filePath.FullName, "content", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        var fileContentBefore = await File.ReadAllBytesAsync(filePath.FullName);
        Assert.NotEqual(new byte[] { 0xEF, 0xBB, 0xBF }, fileContentBefore.Take(3).ToArray());
        Assert.Equal(7, fileContentBefore.Length);

        // Assert that file are not updated, by ensuring that the same file only can be read
        using var _ = File.Open(filePath.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);

        // Act
        await service.RemoveBomFromFileAsync(filePath);

        // Assert
        var fileContentAfter = await File.ReadAllBytesAsync(filePath.FullName);
        Assert.NotEqual(new byte[] { 0xEF, 0xBB, 0xBF }, fileContentAfter.Take(3).ToArray());
        Assert.Equal(7, fileContentAfter.Length);
    }
}
