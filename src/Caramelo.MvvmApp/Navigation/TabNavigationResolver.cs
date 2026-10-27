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

    public static bool ResolveCanClose(Type? viewType, Type? viewModelType, bool defaultCanClose = true)
    {
        // 1. View CanCloseAttribute
        var viewCanCloseAttr = viewType?.GetCustomAttribute<CanCloseAttribute>(inherit: true);
        if (viewCanCloseAttr != null)
            return viewCanCloseAttr.CanClose;

        // 2. View TabNavigationAttribute with explicit CanClose
        var viewTabNavAttr = viewType?.GetCustomAttribute<TabNavigationAttribute>(inherit: true);
        if (viewTabNavAttr is { HasExplicitCanClose: true })
            return viewTabNavAttr.CanClose;

        // 3. ViewModel CanCloseAttribute
        var vmCanCloseAttr = viewModelType?.GetCustomAttribute<CanCloseAttribute>(inherit: true);
        if (vmCanCloseAttr != null)
            return vmCanCloseAttr.CanClose;

        // 4. ViewModel TabNavigationAttribute with explicit CanClose
        var vmTabNavAttr = viewModelType?.GetCustomAttribute<TabNavigationAttribute>(inherit: true);
        return vmTabNavAttr is { HasExplicitCanClose: true } ? vmTabNavAttr.CanClose : defaultCanClose;
    }

    public static bool ResolveCanClose(object? view, object? viewModel, bool defaultCanClose = true)
    {
        return ResolveCanClose(view?.GetType(), viewModel?.GetType(), defaultCanClose);
    }
}
