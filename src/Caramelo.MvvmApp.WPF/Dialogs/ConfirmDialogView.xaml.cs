﻿using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using Caramelo.MvvmApp.WPF.View;

namespace Caramelo.MvvmApp.WPF.Dialogs;

public partial class ConfirmDialogView : MvvmUserControl<ConfirmDialogViewModel>
{
    public ConfirmDialogView()
    {
        InitializeComponent();
    }
}