﻿using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using Caramelo.MvvmApp.WPF.Controls;

namespace Caramelo.MvvmApp.WPF.Dialogs;

public partial class MessageDialogView : MvvmUserControl<MessageDialogViewModel>
{
    public MessageDialogView()
    {
        InitializeComponent();
    }
}