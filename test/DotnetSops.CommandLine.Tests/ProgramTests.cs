namespace DotnetSops.CommandLine.Tests;

public class ProgramTests
{
    private static readonly string[] s_parameters = ["download-sops"];

    [Fact]
    public void EntryPoint_CanExecuteValidCommand()
    {
        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        var exitCode = entryPoint.Invoke(null, [s_parameters]);

        Assert.Equal(0, exitCode);
    }
}
