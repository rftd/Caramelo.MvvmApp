using System.ComponentModel;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Navigation;

public class MvvmNavigateEventArgs(NavigationMode mode) : CancelEventArgs
{
    #region Constructors

    public MvvmNavigateEventArgs(IMvvmViewModel viewModel, NavigationMode mode)
        : this(mode)
    {
        ViewModel = viewModel;
    }

    #endregion Constructors

    #region Properties

    public NavigationMode Mode { get; set; } = mode;

    public IMvvmViewModel? ViewModel { get; set; }

    #endregion Properties
}