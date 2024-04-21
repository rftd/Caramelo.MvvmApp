using System.Reactive;

namespace Caramelo.MvvmApp.ViewModel;

public interface IMvvmSplashViewModel : IMvvmViewModel
{
    IObservable<Unit> WhenFinished { get; }
}