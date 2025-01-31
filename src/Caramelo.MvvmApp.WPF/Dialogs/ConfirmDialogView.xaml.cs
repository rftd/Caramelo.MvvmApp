using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.WPF.Dialogs;

public partial class ConfirmDialogView : MvvmUserControl<ConfirmDialogViewModel>
{
    public ConfirmDialogView()
    {
        InitializeComponent();
    }
}