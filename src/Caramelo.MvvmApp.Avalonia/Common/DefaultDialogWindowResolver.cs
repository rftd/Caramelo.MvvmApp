using Avalonia.Controls;

namespace Caramelo.MvvmApp.Avalonia.Commom;

public class DefaultDialogWindowResolver : IDialogWindowResolver
{
    public Window CreateWindow(Control control)
    {
        var window = new Window
        {
            Content = new DockPanel
            {
                LastChildFill = true,
                Children = { control }
            },
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false
        };

        return window;
    }
}