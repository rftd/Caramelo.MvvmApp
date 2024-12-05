using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        Log = Service.GetRequiredService<ILogger<MvvmViewModel>>();
    }

    #endregion Constructors

    #region Properties

    bool IMvvmViewModel.IsBusy { get; } = false;

    string IMvvmViewModel.Title { get; } = "";
    
    public IServiceProvider Service { get; }
    
    public ILogger Log { get; }

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
        try
        {
            await Task.Delay(1000);
            CloseSplash();
        }
        catch (Exception e)
        {
            Log.LogError(e, "An exception occured during splash appearing");
        }
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