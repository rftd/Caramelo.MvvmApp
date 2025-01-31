using System.Windows;
using System.Windows.Controls;

namespace Caramelo.MvvmApp.WPF.Common;

public interface IDialogWindowResolver
{
    Window CreateWindow(Control control);
}