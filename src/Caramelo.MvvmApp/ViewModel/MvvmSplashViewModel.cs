using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;

namespace Caramelo.MvvmApp.ViewModel;

public abstract class MvvmSplashViewModel : ReactiveObject, IMvvmSplashViewModel
{
    #region Fields

    private Subject<Unit> whenFinishedSubject;

    #endregion Fields

    #region Constructors

    protected MvvmSplashViewModel(IServiceProvider service)
    {
        Service = service;
        whenFinishedSubject = new Subject<Unit>();
    }

    #endregion Constructors

    #region Properties

    bool IMvvmViewModel.IsBusy { get; } = false;

    string IMvvmViewModel.Title { get; } = "";
    
    public IServiceProvider Service { get; }

    public IObservable<Unit> WhenFinished => whenFinishedSubject.AsObservable();
    
    #endregion Properties

    #region Methods

    protected void CloseSplash()
    {
        whenFinishedSubject.OnNext(Unit.Default);
        whenFinishedSubject.OnCompleted();
        whenFinishedSubject.Dispose();
    }

    public virtual void ViewCreated()
    {
    }

    public virtual void ViewAppearing()
    {
    }

    public virtual async void ViewAppeared()
    {
        await Task.Delay(1000);
        CloseSplash();
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