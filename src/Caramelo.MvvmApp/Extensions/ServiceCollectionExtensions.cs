using System.Data;
using System.Data.Common;
using System.Reflection;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReactiveUI;

namespace Caramelo.MvvmApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewAndModelFromAssembly(this IServiceCollection services,
        params Assembly[] extraAssemblies)
    {
        var assemblies = extraAssemblies.Concat([Assembly.GetEntryAssembly()])
            .Where(x => x is not null).Cast<Assembly>().ToArray();
        var viewModel = typeof(IMvvmViewModel);
        var routerModel = typeof(RoutableViewModel);
        var viewModels = assemblies.SelectMany(x => x.DefinedTypes)
            .Where(type => viewModel.IsAssignableFrom(type) &&
                           !routerModel.IsAssignableFrom(type) &&
                           type is { IsAbstract: false, IsInterface: false })
            .ToArray();

        foreach (var model in viewModels)
        {
            var viewType = typeof(IViewFor<>).MakeGenericType(model);
            var view = assemblies.SelectMany(x => x.DefinedTypes).SingleOrDefault(x => viewType.IsAssignableFrom(x));
            if (view == null) continue;

            services.AddTransient(model).AddTransient(viewType, view);
        }

        return services;
    }

    public static IServiceCollection AddViewTransient<TView, TViewModel>(this IServiceCollection services)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IMvvmViewModel
    {
        return services.AddTransient<IViewFor<TViewModel>, TView>().AddTransient<TViewModel>();
    }

    public static IServiceCollection AddViewSingleton<TView, TViewModel>(this IServiceCollection services)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IMvvmViewModel
    {
        return services.AddSingleton<IViewFor<TViewModel>, TView>().AddSingleton<TViewModel>();
    }

    public static IServiceCollection AddViewScoped<TView, TViewModel>(this IServiceCollection services)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IMvvmViewModel
    {
        return services.AddScoped<IViewFor<TViewModel>, TView>().AddScoped<TViewModel>();
    }

    public static IServiceCollection TryAddViewTransient<TView, TViewModel>(this IServiceCollection services)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IMvvmViewModel
    {
        services.TryAddTransient<IViewFor<TViewModel>, TView>();
        services.TryAddTransient<TViewModel>();
        return services;
    }

    public static IServiceCollection TryAddViewSingleton<TView, TViewModel>(this IServiceCollection services)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IMvvmViewModel
    {
        services.TryAddSingleton<IViewFor<TViewModel>, TView>();
        services.TryAddSingleton<TViewModel>();
        return services;
    }

    public static IServiceCollection TryAddViewScoped<TView, TViewModel>(this IServiceCollection services)
        where TView : class, IViewFor<TViewModel>
        where TViewModel : ReactiveObject, IMvvmViewModel
    {
        services.TryAddScoped<IViewFor<TViewModel>, TView>();
        services.TryAddScoped<TViewModel>();
        return services;
    }

    public static IServiceCollection AddDbConnection<TConnection>(this IServiceCollection service,
        string connectionString) where TConnection : DbConnection, IDbConnection
    {
        service.TryAddTransient<TConnection>(_ =>
            (TConnection)(Activator.CreateInstance(typeof(TConnection), connectionString)
                          ?? throw new NotSupportedException()));
        return service;
    }
}