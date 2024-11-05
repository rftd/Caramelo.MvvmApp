using System.Reactive.Linq;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.Services;
using Caramelo.MvvmApp.Services.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Caramelo.MvvmApp.ViewModel;

public abstract class MvvmViewModel : ReactiveObject, IMvvmViewModel, IRoutableViewModel, IActivatableViewModel
{
    #region Fields

    private bool isBusy;
    private string title = string.Empty;

    #endregion Fields

    #region Constructors

    protected MvvmViewModel(IServiceProvider service)
    {
        Service = service;
        Navigation = Service.GetRequiredService<INavigationService>();
        Log = Service.GetRequiredService<ILogger<MvvmViewModel>>();
        Dialogs = Service.GetRequiredService<IDialogService>();
        IsBusy = false;

        ((NavigationService)Navigation).BeforeNavigate.SelectMany(async args =>
        {
            var ret = await CanNavigateAsync(args.Mode, args.ViewModel);
            args.Cancel = !ret;
            return args;
        }).Subscribe();
    }

    #endregion Constructors

    #region Properties

    string? IRoutableViewModel.UrlPathSegment => GetType().Name;

    IScreen IRoutableViewModel.HostScreen { get; } = null!;

    ViewModelActivator IActivatableViewModel.Activator { get; } = new();

    protected INavigationService Navigation { get; }

    public IServiceProvider Service { get; }

    public IDialogService Dialogs { get; }

    public ILogger Log { get; }

    public bool IsBusy
    {
        get => isBusy;
        set => this.RaiseAndSetIfChanged(ref isBusy, value);
    }

    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    #endregion Properties

    #region Methods

    public virtual Task<bool> CanNavigateAsync(NavigationMode mode, IMvvmViewModel? viewModel) => Task.FromResult(true);

    public virtual void ViewCreated()
    {
    }

    public virtual void ViewAppearing()
    {
    }

    public virtual void ViewAppeared()
    {
    }

    public virtual void ViewDisappearing()
    {
    }

    public virtual void ViewDisappeared()
    {
    }

    public virtual void ViewDestroy(bool viewFinishing = true)
    {
    }

    #endregion Methods
}

public abstract class MvvmViewModel<TParameter> : MvvmViewModel, IMvvmViewModelParameter where TParameter : notnull
{
    protected MvvmViewModel(IServiceProvider service) : base(service)
    {
    }

    void IMvvmViewModelParameter.Initialize(object parameter)
    {
        Initialize((TParameter)parameter);
    }

    public abstract void Initialize(TParameter parameter);
}

public abstract class MvvmResultViewModel<TResult> : MvvmViewModel, IMvvmViewModelResult where TResult : notnull
{
    protected MvvmResultViewModel(IServiceProvider service) : base(service)
    {
    }
}

public abstract class MvvmViewModel<TParameter, TResult> : MvvmResultViewModel<TResult>, IMvvmViewModelParameterResult
    where TParameter : notnull
    where TResult : notnull
{
    protected MvvmViewModel(IServiceProvider service) : base(service)
    {
    }

    void IMvvmViewModelParameter.Initialize(object parameter)
    {
        Initialize((TParameter)parameter);
    }

    public abstract void Initialize(TParameter parameter);
}