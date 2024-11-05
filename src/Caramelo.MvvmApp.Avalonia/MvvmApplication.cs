using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Caramelo.MvvmApp.Avalonia.Extensions;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Caramelo.MvvmApp.Avalonia;

public abstract class MvvmApplication<TViewModel> : Application
    where TViewModel : AppViewModel
{
    #region Private Fields

    /// <summary>
    ///     Contains a mutex that is used to prevent an application to open twice.
    /// </summary>
    private Mutex? singleInstanceMutex;

    #endregion Private Fields

    #region Constructors

    /// <summary>
    ///     Destroys this object instance (when the dispose method has already been called the finalization for this object
    ///     instance is suppressed and therefore the finalizer is not called).
    /// </summary>
    ~MvvmApplication()
    {
        // Since the object is being finalized, all other managed resources have already been disposed of, therefore only the unmanaged resources are being disposed of
        Dispose(false);
    }

    #endregion Constructors
    
    #region Properties

    public IClassicDesktopStyleApplicationLifetime DesktopApp =>
        (IClassicDesktopStyleApplicationLifetime) ApplicationLifetime! ?? throw new InvalidOperationException();
    
    public TViewModel? ViewModel
    {
        get => (TViewModel?)DataContext;
        set => DataContext = value;
    }

    #endregion Properties
    
    #region Protected Methods
    
    protected virtual void OnStarted(IEnumerable<string> args, bool isFirstInstance)
    {
        if (!isFirstInstance)
            DesktopApp.Shutdown();
    }
    
    protected virtual void OnExit()
    {
        //Ignored
    }
    
    protected virtual void OnUnhandledExceptionAsync(UnhandledExceptionEventArgs eventArguments)
    {
        //Ignored
    }
    
    #endregion Protected Methods
    
    #region Methods
    
    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        
        if(ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime) return;
        
        AppDomain.CurrentDomain.UnhandledException += (_, eventArguments) => OnUnhandledExceptionAsync(eventArguments);
        DesktopApp.Exit += (_, _) =>
        {
            OnExit();
            Dispose(true);
        };
        
        var mutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName, out var isFirstInstance);

        // If this is the first instance of the application, the mutex is set and stored, so that it can be disposed of when this instance closes
        if (isFirstInstance) singleInstanceMutex = mutex;
        
        ViewModel = MvvmApp.Current.Services.GetRequiredService<TViewModel>();
        
        var viewLocator = MvvmApp.Current.Services.GetRequiredService<IViewLocator>();
        var mainView = viewLocator.ResolveView(ViewModel);

        if(mainView == null) throw new ApplicationException("View principal não encontrada.");
        
        // Calls the on started method where the user is able to call his own code to set up the application
        OnStarted(DesktopApp.Args ?? [], isFirstInstance);
        
        var splashViewModel = MvvmApp.Current.Services.GetService<IMvvmSplashViewModel>();
        if (splashViewModel != null)
        {
            var splashView = viewLocator.ResolveView(splashViewModel);
            if(splashView == null) throw new ApplicationException("SplashView não encontrada.");

            splashView!.ViewModel = splashViewModel;
            DesktopApp.MainWindow = (Window)splashView;
            DesktopApp.MainWindow.Show();

            splashViewModel.WhenFinished.Subscribe(_ =>
            {
                mainView!.ViewModel = ViewModel;
                DesktopApp.MainWindow = (Window)mainView;
                DesktopApp.MainWindow.Show();
            
                ((Window)splashView).Close();
            });
        }
        else
        {
            mainView!.ViewModel = ViewModel;
            DesktopApp.MainWindow = (Window)mainView;
        }
        
        ViewModel.OnFinishApp.Subscribe(DesktopApp.Shutdown);
        ViewModel.OnRestartApp.Subscribe(DesktopApp.Restart);
    }
    
    #endregion Methods
    
    #region IDisposable Implementation

    /// <summary>
    ///     Disposes of all managed and unmanaged resources that have been allocated. This method can be overridden by
    ///     sub-classes in order to implement custom disposal logic.
    /// </summary>
    /// <param name="disposing">
    ///     Determines whether only unmanaged, or managed and unmanaged resources should be disposed of. This is needed when
    ///     the method is called from the destructor, because when the destructor is called all managed resources have already
    ///     been disposed of.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        // Checks if managed resources should be disposed of
        if (!disposing) return;

        singleInstanceMutex?.Dispose();
    }

    /// <summary>
    ///     Disposes of all managed and unmanaged resources that were allocated.
    /// </summary>
    public void Dispose()
    {
        // Calls the virtual dispose method, which may be overridden by sub-classes in order to dispose of their resources
        Dispose(true);

        // Suppresses the finalizer for this object, because all resources have already been disposed of
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Implementation
}