using System.ComponentModel;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Navigation;

public class MvvmNavigateEventArgs : CancelEventArgs
{
    #region Constructors

    public MvvmNavigateEventArgs(NavigationMode mode)
    {
        Mode = mode;
    }

    public MvvmNavigateEventArgs(IMvvmViewModel viewModel, NavigationMode mode)
        : this(mode)
    {
        ViewModel = viewModel;
    }

    #endregion Constructors

    #region Properties

    public NavigationMode Mode { get; set; }
    
    public IMvvmViewModel? ViewModel { get; set; }

    #endregion Properties
}