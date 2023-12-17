using System.Reflection;

namespace DotnetSops.CommandLine.Tests.Extensions;

public static class NSubstituteExtensions
{
    public static T? ProtectedMethod<T>(
        this object obj,
        string methodName,
        params object[] callParams
    )
    {
        ArgumentNullException.ThrowIfNull(obj);
        var method = obj.GetType()
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        var result = method?.Invoke(obj, callParams);
        return (T?)result;
    }
}
