using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Caramelo.MvvmApp.Extensions;

public static class MvvmAppBuilderExtensions
{
    public static MvvmAppBuilder UserSplash<TView, TViewModel>(this MvvmAppBuilder builder)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IMvvmSplashViewModel
    {
        builder.Services.AddTransient<IViewFor<TViewModel>, TView>()
            .AddTransient<IMvvmSplashViewModel, TViewModel>();

        return builder;
    }
}