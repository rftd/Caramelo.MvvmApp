using Avalonia.Controls;

namespace Caramelo.MvvmApp.Avalonia.Commom;

public interface IDialogWindowResolver
{
    Window CreateWindow(Control control);
}