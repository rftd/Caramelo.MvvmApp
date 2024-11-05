﻿using System.Reflection;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReactiveUI;

namespace Caramelo.MvvmApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewAndModelFromAssembly(this IServiceCollection services)
    {
        return services.AddViewAndModelFromAssembly(Assembly.GetExecutingAssembly());
    }
    
    public static IServiceCollection AddViewAndModelFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var viewModel = typeof(IMvvmViewModel);
        var routerModel = typeof(AppViewModel);
        var viewModels = assembly.GetTypes()
            .Where(type => viewModel.IsAssignableFrom(type) && !routerModel.IsAssignableFrom(type) && 
                        type is {IsAbstract: false, IsInterface: false})
            .ToArray();

        foreach (var model in viewModels)
        {
            var viewType = typeof(IViewFor<>).MakeGenericType(model);
            var view = assembly.GetTypes().SingleOrDefault(x => viewType.IsAssignableFrom(x));
            if(view == null) continue;

            services.AddTransient(model).AddTransient(view);
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
}