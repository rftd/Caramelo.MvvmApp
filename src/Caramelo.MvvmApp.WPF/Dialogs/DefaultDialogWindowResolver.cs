using System.Windows;
using System.Windows.Controls;

namespace Caramelo.MvvmApp.WPF.Dialogs;

public class DefaultDialogWindowResolver : IDialogWindowResolver
{
    public Window CreateWindow(Control dialog)
    {
        var window = new Window
        {
            Content = new DockPanel
            {
                LastChildFill = true,
                Children = { dialog }
            },
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.NoResize
        };

        return window;
    }
}