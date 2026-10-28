using System;
using Avalonia;
using ReactiveUI.Avalonia.Reactive;
using Caramelo.MvvmApp.Avalonia.Demo.Common;
using Caramelo.MvvmApp.Avalonia.Demo.Views;
using Caramelo.MvvmApp.Avalonia.Extensions;
using Caramelo.MvvmApp.Demo.Core.ViewModels;
using Caramelo.MvvmApp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Caramelo.MvvmApp.Avalonia.Demo
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main()
        {
            var builder = MvvmApp.CreateBuilder();
            builder.UseAvalonia<App, DemoAppViewModel, DemoAppView>()
                .UseDialogResolver<DialogWindowResolver>()
                .ConfigureAvalonia(x => x.With(new X11PlatformOptions
                {
                    EnableMultiTouch = true,
                    UseDBusMenu = true,
                    EnableIme = true
                }));

            builder.Logging.AddSerilog(new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(applyThemeToRedirectedOutput: true)
                .WriteTo.File("caramelo_avalonia.log", rollingInterval: RollingInterval.Day)
                .CreateLogger());

            // Adiciona as views e os model
            builder.Services.AddViewAndModelFromAssembly(typeof(DemoViewModel).Assembly);
            var app = builder.Build();
            app.Run();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .LogToTrace()
                .UsePlatformDetect()
                .UseReactiveUI(_ => { })
                .With(new X11PlatformOptions
                {
                    EnableMultiTouch = true,
                    UseDBusMenu = true,
                    EnableIme = true
                });
        }
    }
}