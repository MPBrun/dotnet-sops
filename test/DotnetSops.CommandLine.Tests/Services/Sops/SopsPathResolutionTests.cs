using DotnetSops.CommandLine.Services.Sops;
using DotnetSops.CommandLine.Tests.Fixtures;

namespace DotnetSops.CommandLine.Tests.Services.Sops;

[Collection(CollectionNames.UniqueCurrentDirectory)]
public class SopsPathResolutionTests : IDisposable
{
    private readonly UniqueCurrentDirectoryFixture _directory = new();
    private static string ExecutableName => OperatingSystem.IsWindows() ? "sops.exe" : "sops";

    [Fact]
    public void ResolveSopsPath_PathEntriesTakePrecedenceInOrder()
    {
        var first = Directory.CreateDirectory("first").FullName;
        var second = Directory.CreateDirectory("second").FullName;
        var fallback = Directory.CreateDirectory("fallback").FullName;
        var expected = Path.Combine(first, ExecutableName);
        File.WriteAllText(expected, "first");
        File.WriteAllText(Path.Combine(second, ExecutableName), "second");
        File.WriteAllText(Path.Combine(fallback, ExecutableName), "fallback");
        var path = string.Join(Path.PathSeparator, Path.Combine(first, "..", "first"), second);

        var actual = SopsService.ResolveSopsPath(path, fallback);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ResolveSopsPath_MissingPathExecutableUsesDownloadedSops()
    {
        var missing = Directory.CreateDirectory("missing").FullName;
        var fallback = Directory.CreateDirectory("fallback").FullName;
        var expected = Path.Combine(fallback, ExecutableName);
        File.WriteAllText(expected, "fallback");

        var actual = SopsService.ResolveSopsPath(missing, fallback);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ResolveSopsPath_UnsetPathUsesDownloadedSops()
    {
        var fallback = Directory.CreateDirectory("fallback").FullName;
        var expected = Path.Combine(fallback, ExecutableName);
        File.WriteAllText(expected, "fallback");

        var actual = SopsService.ResolveSopsPath(null, fallback);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ResolveSopsPath_RelativeAndEmptyEntriesAreIgnored()
    {
        var relative = Directory.CreateDirectory("relative");
        File.WriteAllText(Path.Combine(relative.FullName, ExecutableName), "relative");
        File.WriteAllText(ExecutableName, "current directory");
        var path = string.Join(Path.PathSeparator, string.Empty, "relative", string.Empty);

        Assert.Throws<SopsMissingException>(() => SopsService.ResolveSopsPath(path, null));
    }

    [Fact]
    public void ResolveSopsPath_NoExecutableThrowsSopsMissingException()
    {
        var missing = Directory.CreateDirectory("missing").FullName;

        var exception = Assert.Throws<SopsMissingException>(() =>
            SopsService.ResolveSopsPath(missing, missing)
        );

        Assert.Equal("SOPS executable could not be found on the PATH.", exception.Message);
    }

    protected virtual void Dispose(bool disposing)
    {
        _directory.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
