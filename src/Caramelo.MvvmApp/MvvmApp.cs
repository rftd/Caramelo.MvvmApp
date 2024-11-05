using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;

namespace Caramelo.MvvmApp;

public sealed class MvvmApp : IDisposable
{
    #region Fields

    private readonly IMvvmApplication mvvmApplication;

    #endregion Fields

    #region Constructors

    internal MvvmApp(IServiceProvider services)
    {
        Current = this;
        Services = services;
        Configuration = services.GetRequiredService<IConfiguration>();
        mvvmApplication = services.GetRequiredService<IMvvmApplication>();
        InitializeReactiveUi();
    }
    
    ~MvvmApp()
    {
        Dispose();
    }

    #endregion Constructors

    #region Properties

    public static MvvmApp Current { get; private set; } = null!;

    /// <summary>The application's configured services.</summary>
    public IServiceProvider Services { get; }

    /// <summary>
    ///     The application's configured <see cref="T:Microsoft.Extensions.Configuration.IConfiguration" />.
    /// </summary>
    public IConfiguration Configuration { get; }

    #endregion Properties

    #region Methods

    private void InitializeReactiveUi()
    {
        Locator.CurrentMutable.InitializeReactiveUI();
        Locator.CurrentMutable.RegisterLazySingleton(() => Services.GetRequiredService<IViewLocator>());
    }

    public static MvvmAppBuilder CreateBuilder() => new();
    
    public void Run() => mvvmApplication.Run();

    public void Dispose()
    {
        (Configuration as IDisposable)?.Dispose();
        (Services as IDisposable)?.Dispose();
        mvvmApplication.Dispose();

        // Take this object off the finalization queue and prevent finalization code for this
        // object from executing a second time.
        GC.SuppressFinalize(this);
    }

    #endregion
}