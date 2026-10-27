using System;
using Avalonia;
using Caramelo.MvvmApp.Avalonia.Commom;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Avalonia.Reactive;

namespace Caramelo.MvvmApp.Avalonia;

internal class AvaloniaApplication<TApp, TView> : IMvvmApplication
    where TApp : MvvmApplication<TView>, new()
    where TView : AppViewModel
{
    public int Run()
    {
        var builder = AppBuilder.Configure<TApp>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI(_ => { });

        var configuration = MvvmApp.Current.Services.GetService<IAvaloniaConfiguration>();
        configuration?.Configure(builder);
        
        return builder.StartWithClassicDesktopLifetime(Environment.GetCommandLineArgs());
    }
    
    public void Dispose() => (Application.Current as MvvmApplication<TView>)?.Dispose();
}