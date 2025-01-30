using Avalonia.Controls;

namespace Caramelo.MvvmApp.Avalonia.Dialogs;

public interface IDialogWindowResolver
{
    Window CreateWindow(Control control);
}