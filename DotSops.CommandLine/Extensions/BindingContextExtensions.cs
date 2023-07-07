using System.CommandLine.Binding;

namespace DotSops.CommandLine.Extensions;
internal static class BindingContextExtensions
{
    public static T GetService<T>(this BindingContext bindingContext) where T : notnull
    {
        return (T)bindingContext.GetService(typeof(T))!;
    }
}
