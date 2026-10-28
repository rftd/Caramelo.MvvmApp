using ActiproSoftware.UI.Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Data;
using Caramelo.MvvmApp.Avalonia.Commom;
using Caramelo.MvvmApp.Avalonia.Demo.Controls;

namespace Caramelo.MvvmApp.Avalonia.Demo.Common;

public sealed class DialogWindowResolver : IDialogWindowResolver
{
    public Window CreateWindow(Control control)
    {
        var caption = new DefaultCaption();
        var titleBar = new ChromedTitleBar
        {
            LeftContent = caption,
            IsMaximizeButtonAllowed = false,
            IsMinimizeButtonAllowed = false,
            IsCloseButtonAllowed = false
        };
        DockPanel.SetDock(titleBar, Dock.Top);

        var window = new Window
        {
            WindowDecorations = WindowDecorations.None,
            Content = new DockPanel
            {
                LastChildFill = true,
                Children = { titleBar, control }
            },
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false
        };

        caption.Bind(DefaultCaption.TitleProperty, new Binding("Title") { Source = window });
        return window;
    }
}
