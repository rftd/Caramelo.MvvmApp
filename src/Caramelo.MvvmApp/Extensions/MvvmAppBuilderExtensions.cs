using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Reactive;

namespace Caramelo.MvvmApp.Extensions;

public static class MvvmAppBuilderExtensions
{
    public static MvvmAppBuilder UseSplash<TView, TViewModel>(this MvvmAppBuilder builder)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IMvvmSplashViewModel
    {
        builder.Services.AddTransient<IViewFor<TViewModel>, TView>()
            .AddTransient<IMvvmSplashViewModel, TViewModel>();

        return builder;
    }
}