using Caramelo.MvvmApp.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.ViewModel;

public abstract partial class MvvmDialogViewModel<TParameter, TResult> : ReactiveObject, 
    IMvvmViewModel, IMvvmDialogViewModel<TParameter, TResult> 
    where TParameter : DialogOptions
{
    #region Fields

    [Reactive] private bool isBusy;
    [Reactive] private string title = string.Empty;
    [Reactive] private bool canClose;
    [Reactive] private TResult dialogResult;

    #endregion Fields

    #region Constructors

    protected MvvmDialogViewModel(IServiceProvider service)
    {
        Service = service;
        Log = Service.GetRequiredService<ILogger<MvvmDialogViewModel<TParameter, TResult>>>();
        IsBusy = false;
        DialogResult = default!;
    }

    #endregion Constructors
    
    #region Properties

    public IServiceProvider Service { get; }
    
    public ILogger Log { get; }

    #endregion Properties
    
    #region Methods

    public abstract void Initialize(TParameter parameter);

    protected void SetResult(TResult result)
    {
        dialogResult = result;
        CanClose = true;
    }
    
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