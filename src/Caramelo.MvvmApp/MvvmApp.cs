using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;

namespace Caramelo.MvvmApp;

public sealed class MvvmApp : MvvmDisposable
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

    protected override void DisposeManaged()
    {
        (Configuration as IDisposable)?.Dispose();
        (Services as IDisposable)?.Dispose();
        mvvmApplication.Dispose();
    }

    #endregion
}