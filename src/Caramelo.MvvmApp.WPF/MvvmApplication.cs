using System.Reflection;
using System.Windows;
using Caramelo.MvvmApp.ViewModel;
using Caramelo.MvvmApp.WPF.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Caramelo.MvvmApp.WPF;

/// <summary>
///     Represents the base class for applications based on the MVVM pattern.
/// </summary>
public abstract class MvvmApplication<TViewModel> : Application, IMvvmApplication where TViewModel : AppViewModel
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
    
    #region Protected Methods

    /// <summary>
    ///     Gets called after the application startup. This can be overridden by the user to implement custom startup logic and
    ///     displaying views.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="isFirstInstance"></param>
    /// <param name="appBootstrapper"></param>
    protected virtual void OnStarted(IEnumerable<string> args, bool isFirstInstance, TViewModel appBootstrapper)
    {
        if (!isFirstInstance) Shutdown(0);
    }

    /// <summary>
    ///     Gets called right before the application quits. This can be overridden by the user to implement custom shutdown
    ///     logic.
    /// </summary>
    protected virtual void OnExit()
    {
        //Ignored
    }

    /// <summary>
    ///     Gets called if an exception was thrown that was not handled by user-code.
    /// </summary>
    /// <param name="eventArguments">
    ///     The event arguments that contain further information about the exception that was not
    ///     properly handled by user-code.
    /// </param>
    protected virtual void OnUnhandledExceptionAsync(UnhandledExceptionEventArgs eventArguments)
    {
        //Ignored
    }

    #endregion Protected Methods

    #region Application Implementation

    /// <summary>
    ///     Is called once the application has started.
    /// </summary>
    /// <param name="e">The event arguments that contain more information about the application startup.</param>
    protected sealed override void OnStartup(StartupEventArgs e)
    {
        // Calls the base implementation of this method
        base.OnStartup(e);

        // Signs up for the unhandled exception event, which is raised when an exception was thrown, which was not handled by user-code
        AppDomain.CurrentDomain.UnhandledException += (_, eventArguments) => OnUnhandledExceptionAsync(eventArguments);

        // Determines whether this application instance is the first instance of the application or whether another instance is already running (this can be used to force the application to be a single instance application)
        var mutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName, out var isFirstInstance);

        // If this is the first instance of the application, the mutex is set and stored, so that it can be disposed of when this instance closes
        if (isFirstInstance) singleInstanceMutex = mutex;

        var appViewModel = MvvmApp.Current.Services.GetRequiredService<TViewModel>();
        var viewLocator = MvvmApp.Current.Services.GetRequiredService<IViewLocator>();
        var mainView = viewLocator.ResolveView(appViewModel);
        if(mainView == null) throw new ApplicationException("View principal não encontrada.");
        
        // Calls the on started method where the user is able to call his own code to set up the application
        OnStarted(e.Args, isFirstInstance, appViewModel);
        
        var splashViewModel = MvvmApp.Current.Services.GetService<IMvvmSplashViewModel>();
        if (splashViewModel != null)
        {
            var splashView = viewLocator.ResolveView(splashViewModel);
            if(splashView == null) throw new ApplicationException("SplashView não encontrada.");

            splashView!.ViewModel = splashViewModel;
            MainWindow = (Window)splashView;
            MainWindow.Show();

            splashViewModel.WhenFinished.Subscribe(_ =>
            {
                mainView!.ViewModel = appViewModel;
                MainWindow = (Window)mainView;
                MainWindow.Show();
            
                ((Window)splashView).Close();
            });
        }
        else
        {
            mainView!.ViewModel = appViewModel;
            MainWindow = (Window)mainView;
            MainWindow.Show();
        }
        
        appViewModel.OnFinishApp.Subscribe(Shutdown);
        appViewModel.OnRestartApp.Subscribe(this.Restart);
    }

    /// <summary>
    ///     Is called just before the application is shut down.
    /// </summary>
    /// <param name="e">The event arguments that contain more information about the application shutdown.</param>
    protected sealed override void OnExit(ExitEventArgs e)
    {
        // Calls the base implementation of this method
        base.OnExit(e);

        // Calls the on exit event handler, where the user is able to do custom shutdown operations
        OnExit();

        // Calls the dispose method of the application
        Dispose(true);
    }

    #endregion Application Implementation

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