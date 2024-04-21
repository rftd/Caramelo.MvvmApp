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

namespace Caramelo.MvvmApp;

public sealed class MvvmAppBuilder
{
    #region Fields

    private ILoggingBuilder? logging;

    #endregion Fields

    #region Constructors

    internal MvvmAppBuilder()
    {
        Services = new ServiceCollection();

        Configuration = new ConfigurationManager();
        Services.AddSingleton<IConfiguration>(Configuration);
        
        InitializeNavigation();
        InitializeLog();
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
    }
    
    private void InitializeLog()
    {
        Logging.AddFilter<NullLoggerProvider>(level => level >= LogLevel.Warning);
        if(OperatingSystem.IsWindows())
            Logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);

        Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddJsonFile("appsettings.Debug.json", optional: true, reloadOnChange: true);
    }
    
    public MvvmApp Build()
    {
        // By default, if no one else has configured logging, add a "no-op" LoggerFactory
        // and Logger services with no providers. This way when components try to get an
        // ILogger<> from the IServiceProvider, they don't get 'null'.
        Services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, NullLoggerFactory>());
        Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

        Services.MakeReadOnly();
        
        return new MvvmApp(Services.BuildServiceProvider());
    }

    #endregion
}