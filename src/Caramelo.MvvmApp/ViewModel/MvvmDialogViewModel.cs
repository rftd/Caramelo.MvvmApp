using System.Reactive.Linq;
using System.Reactive.Subjects;
using Caramelo.MvvmApp.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Caramelo.MvvmApp.ViewModel;

public abstract class MvvmDialogViewModel<TParameter, TResult> : ReactiveObject, IMvvmViewModel, IMvvmDialogViewModel<TParameter, TResult> where TParameter : DialogOptions
    where TResult : notnull
{
    #region Fields

    private bool isBusy;
    private string title = string.Empty;
    private bool canClose;
    private TResult dialogResult;

    #endregion Fields

    #region Constructors

    protected MvvmDialogViewModel(IServiceProvider service)
    {
        Service = service;
        Log = Service.GetRequiredService<ILogger<MvvmDialogViewModel<TParameter, TResult>>>();
        IsBusy = false;
    }

    #endregion Constructors
    
    #region Properties

    public IServiceProvider Service { get; }
    
    public ILogger Log { get; }
    
    public TResult DialogResult
    {
        get => dialogResult;
        set => this.RaiseAndSetIfChanged(ref dialogResult, value);
    }
    
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
    
    public bool CanClose
    {
        get => canClose;
        set => this.RaiseAndSetIfChanged(ref canClose, value);
    }

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