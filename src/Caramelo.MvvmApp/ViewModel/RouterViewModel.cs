using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Caramelo.MvvmApp.Services.Impl;
using ReactiveUI;

namespace Caramelo.MvvmApp.ViewModel;

public abstract class RouterViewModel : MvvmViewModel, IScreen
{
    #region Fields

    private Subject<int> finishApp;

    #endregion Fields
    
    #region Constructors

    protected RouterViewModel(IServiceProvider service) : base(service)
    {
        Router = ((NavigationService)Navigation).Router;
        finishApp = new Subject<int>();
        FinishAppCommand = ReactiveCommand.Create(() => FinishApp());
    }

    #endregion Constructors

    #region Properties

    public RoutingState Router { get; }
    
    public IObservable<int> OnFinishApp => finishApp.AsObservable();

    public ReactiveCommand<Unit, Unit> FinishAppCommand { get; protected set; }
    
    #endregion Properties

    #region Methods

    protected void FinishApp(int code = 0)
    {
        finishApp.OnNext(code);
        finishApp.OnCompleted();
        finishApp.Dispose();
    }

    #endregion Methods
}