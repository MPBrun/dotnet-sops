namespace DotnetSops.CommandLine.Tests.Fixtures;

public class UniqueCurrentDirectoryFixture : IDisposable
{
    public static readonly string OriginalCurretDirectory = Directory.GetCurrentDirectory();

    public DirectoryInfo TestDirectory { get; set; }

    public UniqueCurrentDirectoryFixture()
    {
        TestDirectory = Directory.CreateDirectory(
            Path.Combine(OriginalCurretDirectory, "unittest", Guid.NewGuid().ToString())
        );
        Directory.SetCurrentDirectory(TestDirectory.FullName);
    }

    protected virtual void Dispose(bool disposing)
    {
        Directory.SetCurrentDirectory(OriginalCurretDirectory);
        Directory.Delete(TestDirectory.FullName, true);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
