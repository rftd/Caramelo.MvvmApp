namespace Caramelo.MvvmApp.Navigation;

[AttributeUsage(AttributeTargets.Class)]
public class TabNavigationAttribute : Attribute
{
    public TabNavigationMode Mode { get; }
    
    public bool HasExplicitCanClose { get; private set; }

    public bool CanClose
    {
        get;
        set
        {
            field = value;
            HasExplicitCanClose = true;
        }
    } = true;

    public TabNavigationAttribute(TabNavigationMode mode = TabNavigationMode.CurrentTab)
    {
        Mode = mode;
    }

    public TabNavigationAttribute(TabNavigationMode mode, bool canClose)
    {
        Mode = mode;
        CanClose = canClose;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class NewTabAttribute : TabNavigationAttribute
{
    public NewTabAttribute() : base(TabNavigationMode.NewTab) { }
    public NewTabAttribute(bool canClose) : base(TabNavigationMode.NewTab, canClose) { }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class CurrentTabAttribute : TabNavigationAttribute
{
    public CurrentTabAttribute() : base(TabNavigationMode.CurrentTab) { }
    public CurrentTabAttribute(bool canClose) : base(TabNavigationMode.CurrentTab, canClose) { }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class CanCloseAttribute(bool canClose = true) : Attribute
{
    public bool CanClose { get; } = canClose;
}
