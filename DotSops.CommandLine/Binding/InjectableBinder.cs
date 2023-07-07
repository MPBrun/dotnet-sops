using System.CommandLine.Binding;
using DotSops.CommandLine.Extensions;

namespace DotSops.CommandLine.Binding;
internal class InjectableBinder<T> : BinderBase<T> where T : notnull
{
    protected override T GetBoundValue(BindingContext bindingContext)
    {
        return bindingContext.GetService<T>();
    }
}
