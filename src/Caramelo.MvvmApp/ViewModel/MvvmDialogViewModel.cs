using System.Reactive.Linq;
using System.Reactive.Subjects;
using Caramelo.MvvmApp.Dialogs;
using ReactiveUI;

namespace Caramelo.MvvmApp.ViewModel;

public abstract class MvvmDialogViewModel<TParameter, TResult> : ReactiveObject, IMvvmViewModel, IMvvmDialogViewModel<TParameter, TResult> where TParameter : DialogOptions
    where TResult : notnull
{
    #region Fields

    private bool isBusy;
    private string title = string.Empty;
    private bool canClose;
    private Subject<TResult> dialogResult;

    #endregion Fields

    #region Constructors

    protected MvvmDialogViewModel(IServiceProvider service)
    {
        Service = service;
        IsBusy = false;
        dialogResult = new Subject<TResult>();
    }

    #endregion Constructors
    
    #region Properties

    public IServiceProvider Service { get; }
    
    public IObservable<TResult> DialogResult => dialogResult.AsObservable();
    
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
        dialogResult.OnNext(result);
        dialogResult.OnCompleted();
        dialogResult.Dispose();
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