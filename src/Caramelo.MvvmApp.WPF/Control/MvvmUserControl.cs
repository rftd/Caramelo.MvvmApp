﻿using System.ComponentModel;
using System.Windows.Controls;
using Caramelo.MvvmApp.ViewModel;
using ReactiveUI;

namespace Caramelo.MvvmApp.WPF;

public abstract class MvvmUserControl<TViewModel> : UserControl, IViewFor<TViewModel> 
    where TViewModel : ReactiveObject, IMvvmViewModel
{
    #region Fields

    private bool unloaded;

    #endregion Fields

    #region Constructors

    protected MvvmUserControl()
    {
        Loaded += (_, _) =>
        {
            ViewModel?.ViewAppearing();
            ViewModel?.ViewAppeared();
        };

        Unloaded += (_, _) =>
        {
            if (unloaded) return;

            ViewModel?.ViewDisappearing();
            ViewModel?.ViewDisappeared();
            ViewModel?.ViewDestroy();
            unloaded = true;
        };
    }

    #endregion Constructors

    #region Properties

    /// <inheritdoc />
    public TViewModel? ViewModel
    {
        get => (TViewModel) DataContext;
        set => DataContext = value;
    }

    /// <inheritdoc />
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?) value;
    }

    public bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(this);

    #endregion Properties
}