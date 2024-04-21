using System;
using Avalonia;
using Caramelo.MvvmApp.Avalonia.Dialogs;
using Caramelo.MvvmApp.Avalonia.Services;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.Extensions;
using Caramelo.MvvmApp.Services;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReactiveUI;

namespace Caramelo.MvvmApp.Avalonia.Extensions;

public static class MvvmAppBuilderExtension
{
    public static MvvmAppBuilder UseAvalonia<TApp, TView, TMainWindow>(this MvvmAppBuilder builder, Action<AppBuilder> configure)
        where TApp : MvvmApplication<TView>, new()
        where TView : RouterViewModel
        where TMainWindow : class, IViewFor<TView>
    {
        builder.UseAvalonia<TApp, TView, TMainWindow>();

        builder.Services.AddSingleton<IAvaloniaConfiguration>(_ => new AvaloniaConfiguration(configure));
        
        return builder;
    }
    
    public static MvvmAppBuilder UseAvalonia<TApp, TView, TMainWindow>(this MvvmAppBuilder builder)
        where TApp : MvvmApplication<TView>, new()
        where TView : RouterViewModel
        where TMainWindow : class, IViewFor<TView>
    {
        builder.Services.AddSingleton<IMvvmApplication, AvaloniaApplication<TApp, TView>>();
        builder.Services.AddViewSingleton<TMainWindow, TView>();
        
        
        // Add Dialogs
        builder.Services.TryAddSingleton<IDialogService, DialogsService>();
        builder.Services.TryAddViewTransient<MessageDialogView, MessageDialogViewModel>();
        builder.Services.TryAddViewTransient<ConfirmDialogView, ConfirmDialogViewModel>();
        builder.Services.TryAddViewTransient<InputDialogView, InputDialogViewModel>();
        
        return builder;
    }
}