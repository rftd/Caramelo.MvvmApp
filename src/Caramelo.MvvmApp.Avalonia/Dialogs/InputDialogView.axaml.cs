using Caramelo.MvvmApp.Avalonia.Controls;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Avalonia.Dialogs;

public partial class InputDialogView : MvvmUserControl<InputDialogViewModel>
{
    public InputDialogView()
    {
        InitializeComponent();
    }
}