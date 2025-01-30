using System.Diagnostics;
using Caramelo.MvvmApp.Services;
using Caramelo.MvvmApp.Services.Impl;
using Caramelo.MvvmApp.View;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.EventLog;
using ReactiveUI;
using Splat.Microsoft.Extensions.DependencyInjection;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Caramelo.MvvmApp;

public sealed class MvvmAppBuilder
{
    #region Fields

    private ILoggingBuilder? logging;

    #endregion Fields

    #region Constructors

    internal MvvmAppBuilder()
    {
        Configuration = new ConfigurationManager();
        
        Services = new ServiceCollection();
        Services.UseMicrosoftDependencyResolver();
        Services.AddSingleton<IConfiguration>(Configuration);
        
        InitializeNavigation();
        InitializeConfiguration();
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    ///     A collection of services for the application to compose. This is useful for adding user provided or framework
    ///     provided services.
    /// </summary>
    public ServiceCollection Services { get; }

    /// <summary>
    ///     A collection of configuration providers for the application to compose. This is useful for adding new configuration
    ///     sources and providers.
    /// </summary>
    public ConfigurationManager Configuration { get; }

    /// <summary>
    ///     A collection of logging providers for the application to compose. This is useful for adding new logging providers.
    /// </summary>
    public ILoggingBuilder Logging
    {
        get
        {
            return logging ??= InitializeLogging();

            ILoggingBuilder InitializeLogging()
            {
                // if someone accesses the Logging builder, ensure Logging has been initialized.
                Services.AddLogging();
                return new LoggingBuilder(Services);
            }
        }
    }

    #endregion Properties

    #region Methods
    
    private void InitializeNavigation()
    {
        Services.AddSingleton<INavigationService, NavigationService>();
        Services.AddSingleton<IViewLocator, MvvmViewLocator>();
        Services.AddSingleton<IRouterService, RouterService>();
    }

    private void InitializeConfiguration()
    {
        Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Configuration.AddJsonFile(Debugger.IsAttached ? "appsettings.Debug.json" : "appsettings.Release.json",
            optional: true, reloadOnChange: true);
    }
    
    public MvvmApp Build()
    {
        return new MvvmApp(Services.BuildServiceProvider());
    }

    #endregion
}