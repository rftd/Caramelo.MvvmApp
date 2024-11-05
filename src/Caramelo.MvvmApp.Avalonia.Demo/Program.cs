using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Caramelo.MvvmApp.Avalonia.Demo.Views;
using Caramelo.MvvmApp.Avalonia.Extensions;
using Caramelo.MvvmApp.Demo.Core.ViewModels;
using Caramelo.MvvmApp.Extensions;

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
            builder.UseAvalonia<App, AppBootstrapperViewModel, AvaloniaBootstrapperView>()
                .ConfigureAvalonia(x => x.With(new X11PlatformOptions
                {
                    EnableMultiTouch = true,
                    UseDBusMenu = true,
                    EnableIme = true
                }));

            // Adiciona as views e os model
            builder.Services.AddViewAndModelFromAssembly();

            var app = builder.Build();
            app.Run();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .LogToTrace()
                .UsePlatformDetect()
                .UseReactiveUI()
                .With(new X11PlatformOptions
                {
                    EnableMultiTouch = true,
                    UseDBusMenu = true,
                    EnableIme = true
                });
        }
    }
}