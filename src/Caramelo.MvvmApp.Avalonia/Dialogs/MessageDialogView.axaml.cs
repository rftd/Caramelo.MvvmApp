using Caramelo.MvvmApp.Avalonia.Controls;
using Caramelo.MvvmApp.Dialogs;

namespace Caramelo.MvvmApp.Avalonia.Dialogs;

public partial class MessageDialogView : MvvmUserControl<MessageDialogViewModel>
{
    public MessageDialogView()
    {
        InitializeComponent();
    }
}