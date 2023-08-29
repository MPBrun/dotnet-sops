namespace DotnetSops.CommandLine.Tests;
public class ProgramTests
{
    [Fact]
    public void EntryPoint_CanExecuteValidCommand()
    {
        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var exitCode = entryPoint.Invoke(null, new object[] { new string[] { "download-sops" } });

        Assert.Equal(0, exitCode);
    }
}
