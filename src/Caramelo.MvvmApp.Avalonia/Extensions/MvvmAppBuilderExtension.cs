using System;
using System.Linq;
using Avalonia;
using Caramelo.MvvmApp.Avalonia.Commom;
using Caramelo.MvvmApp.Avalonia.Dialogs;
using Caramelo.MvvmApp.Avalonia.Services;
using Caramelo.MvvmApp.Extensions;
using Caramelo.MvvmApp.Services;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReactiveUI;

namespace Caramelo.MvvmApp.Avalonia.Extensions;

public static class MvvmAppBuilderExtension
{
    public static MvvmAppBuilder UseAvalonia<TApp, TView, TMainWindow>(this MvvmAppBuilder builder)
        where TApp : MvvmApplication<TView>, new()
        where TView : AppViewModel
        where TMainWindow : class, IViewFor<TView>
    {
        builder.Services.AddSingleton<IMvvmApplication, AvaloniaApplication<TApp, TView>>();
        builder.Services.AddViewSingleton<TMainWindow, TView>();
        
        
        // Add Dialogs
        builder.Services.TryAddSingleton<IDialogService, DialogsService>();
        builder.Services.TryAddSingleton<IDialogWindowResolver, DefaultDialogWindowResolver>();
        builder.Services.TryAddViewTransient<MessageDialogView, MessageDialogViewModel>();
        builder.Services.TryAddViewTransient<ConfirmDialogView, ConfirmDialogViewModel>();
        builder.Services.TryAddViewTransient<InputDialogView, InputDialogViewModel>();
        
        return builder;
    }

    public static MvvmAppBuilder UseDialogResolver<TResolver>(this MvvmAppBuilder builder)
        where TResolver : class, IDialogWindowResolver
    {
        var descriptorToRemove = builder.Services .FirstOrDefault(d => d.ServiceType == typeof(IDialogWindowResolver));
        if (descriptorToRemove != null)
            builder.Services.Remove(descriptorToRemove);
        
        builder.Services.TryAddSingleton<IDialogWindowResolver, TResolver>();
        return builder;
    }

    public static MvvmAppBuilder ConfigureAvalonia(this MvvmAppBuilder builder, Action<AppBuilder> configure)
    {
        builder.Services.AddSingleton<IAvaloniaConfiguration>(_ => new AvaloniaConfiguration(configure));
        return builder;
    }
}