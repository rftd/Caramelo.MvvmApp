namespace Caramelo.MvvmApp.Navigation;

[AttributeUsage(AttributeTargets.Class)]
public class TabNavigationAttribute(TabNavigationMode mode = TabNavigationMode.CurrentTab) : Attribute
{
    public TabNavigationMode Mode { get; } = mode;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class NewTabAttribute() : TabNavigationAttribute(TabNavigationMode.NewTab);

[AttributeUsage(AttributeTargets.Class)]
public sealed class CurrentTabAttribute() : TabNavigationAttribute();
