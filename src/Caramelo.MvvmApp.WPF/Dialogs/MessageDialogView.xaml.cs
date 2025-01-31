using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.WPF.Dialogs;

public partial class MessageDialogView : MvvmUserControl<MessageDialogViewModel>
{
    public MessageDialogView()
    {
        InitializeComponent();
    }
}