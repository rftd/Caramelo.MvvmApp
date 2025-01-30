using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.Extensions;
using Caramelo.MvvmApp.Services;
using Caramelo.MvvmApp.ViewModel;
using Caramelo.MvvmApp.WPF.Dialogs;
using Caramelo.MvvmApp.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReactiveUI;

namespace Caramelo.MvvmApp.WPF.Extensions;

public static class MvvmAppBuilderExtension
{
    public static MvvmAppBuilder UseWpf<TApp, TView, TMainWindow>(this MvvmAppBuilder builder)
        where TApp : MvvmApplication<TView>, IMvvmApplication
        where TView : AppViewModel
        where TMainWindow : class, IViewFor<TView>
    {
        builder.Services.AddSingleton<IMvvmApplication, TApp>();
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
}