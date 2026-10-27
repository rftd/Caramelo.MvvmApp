using System.Reflection;

namespace Caramelo.MvvmApp.Navigation;

public static class TabNavigationResolver
{
    public static TabNavigationMode ResolveMode(Type? viewType, Type? viewModelType)
    {
        var viewAttribute = viewType?.GetCustomAttribute<TabNavigationAttribute>(inherit: true);
        if (viewAttribute != null)
            return viewAttribute.Mode;

        var vmAttribute = viewModelType?.GetCustomAttribute<TabNavigationAttribute>(inherit: true);
        return vmAttribute?.Mode ?? TabNavigationMode.CurrentTab;
    }

    public static TabNavigationMode ResolveMode(object? view, object? viewModel)
    {
        return ResolveMode(view?.GetType(), viewModel?.GetType());
    }
}
