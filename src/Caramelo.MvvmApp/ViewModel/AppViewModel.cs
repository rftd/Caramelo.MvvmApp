﻿using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Caramelo.MvvmApp.ViewModel;

public abstract class AppViewModel : RoutableViewModel
{
    #region Fields

    private readonly Subject<int> finishApp;
    private readonly Subject<string[]> restartApp;

    #endregion Fields
    
    #region Constructors

    protected AppViewModel(IServiceProvider service) : base(service)
    {
        finishApp = new Subject<int>();
        restartApp = new Subject<string[]>();
    }

    #endregion Constructors

    #region Properties
    
    public IObservable<int> OnFinishApp => finishApp.AsObservable();
    
    public IObservable<string[]> OnRestartApp => restartApp.AsObservable();
    
    #endregion Properties

    #region Methods

    protected void FinishApp(int code = 0)
    {
        finishApp.OnNext(code);
        finishApp.OnCompleted();
        finishApp.Dispose();
    }
    
    protected void RestartApp()
    {
        var args = Environment.GetCommandLineArgs().ToList();
        args.RemoveAt(0);
        
        restartApp.OnNext(args.ToArray());
        restartApp.OnCompleted();
        restartApp.Dispose();
    }

    #endregion Methods
}