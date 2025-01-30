using System.Windows;
using System.Windows.Controls;

namespace Caramelo.MvvmApp.WPF.Dialogs;

public interface IDialogWindowResolver
{
    Window CreateWindow(Control control);
}