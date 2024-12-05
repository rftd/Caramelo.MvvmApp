using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Services.Impl;

internal class NavigationService(IServiceProvider service) : INavigationService
{
    #region Fields

    private readonly Subject<MvvmNavigateEventArgs> beforeNavigate = new();
    private readonly ConditionalWeakTable<MvvmViewModel, TaskCompletionSource<object>?> tcsResults = new();
    private readonly IRouterService routerService = service.GetRequiredService<IRouterService>();

    #endregion Fields

    #region Properties

    public IObservable<MvvmNavigateEventArgs> BeforeNavigate => beforeNavigate.AsObservable();

    #endregion Properties

    #region Methods

    public Task GoToAsync<TModel>() where TModel : MvvmViewModel
    {
        var view = service.GetRequiredService<TModel>();
        
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);
        beforeNavigate.OnNext(args);

        return args.Cancel ? Task.CompletedTask : routerService.GetDefaultRouter().Navigate.Execute(view).ToTask();
    }

    public async Task<TResult?> GoToAsync<TModel, TResult>()
        where TModel : MvvmResultViewModel<TResult> where TResult : notnull
    {
        
        var view = service.GetRequiredService<TModel>();
        
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);
        beforeNavigate.OnNext(args);

        if (args.Cancel)
            return default;
        
        var tcs = new TaskCompletionSource<TResult>();
        tcsResults.Add(view, tcs as TaskCompletionSource<object>);
        
        await routerService.GetDefaultRouter().Navigate.Execute(view).ToTask();

        try
        {
            return await tcs.Task;
        }
        catch (Exception)
        {
            return default!;
        }
    }

    public Task GoToAsync<TModel, TParameter>(TParameter parameter)
        where TModel : MvvmViewModel<TParameter> where TParameter : notnull
    {
        var view = service.GetRequiredService<TModel>();
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);
        beforeNavigate.OnNext(args);

        if (args.Cancel)
            return Task.CompletedTask;
        
        view.Initialize(parameter);
        return routerService.GetDefaultRouter().Navigate.Execute(view).ToTask();
    }

    public async Task<TResult?> GoToAsync<TModel, TParameter, TResult>(TParameter parameter)
        where TModel : MvvmViewModel<TParameter, TResult> where TParameter : notnull where TResult : notnull
    {
        var view = service.GetRequiredService<TModel>();
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);
        beforeNavigate.OnNext(args);

        if (args.Cancel)
            return default;
        
        
        var tcs = new TaskCompletionSource<TResult>();
        tcsResults.Add(view, tcs as TaskCompletionSource<object>);
        view.Initialize(parameter);
        
        await routerService.GetDefaultRouter().Navigate.Execute(view).ToTask();

        try
        {
            return await tcs.Task;
        }
        catch (Exception)
        {
            return default!;
        }
    }

    public Task<bool> CanGoBack(IMvvmViewModel? viewModel = null)
    {
        return CanNavigate(NavigationMode.Close, viewModel);
    }

    public Task<bool> CanGoTo(IMvvmViewModel? viewModel = null)
    {
        return CanNavigate(NavigationMode.Show, viewModel);
    }

    public Task GoBackAsync<TModel>(TModel viewModel) where TModel : MvvmViewModel
    {
        var args = new MvvmNavigateEventArgs(viewModel, NavigationMode.Close);
        beforeNavigate.OnNext(args);
        
        return args.Cancel ? Task.CompletedTask : routerService.GetDefaultRouter().NavigateBack.Execute(Unit.Default).ToTask();
    }

    public Task GoBackAsync<TParameter, TResult>(MvvmViewModel<TParameter, TResult> viewModel, TResult result)
        where TParameter : notnull where TResult : notnull
    {
        var args = new MvvmNavigateEventArgs(viewModel, NavigationMode.Close);
        beforeNavigate.OnNext(args);
        
        if (args.Cancel)
            return Task.CompletedTask;
        
        tcsResults.TryGetValue(viewModel, out var tcs);

        try
        {
            tcs?.TrySetResult(result);
            tcsResults.Remove(viewModel);
            return GoBackAsync(viewModel);
        }
        catch (Exception ex)
        {
            tcs?.TrySetException(ex);
            return Task.FromException(ex);
        }
    }

    public Task GoBackAsync<TResult>(MvvmResultViewModel<TResult> viewModel, TResult result) where TResult : notnull
    {
        var args = new MvvmNavigateEventArgs(viewModel, NavigationMode.Close);
        beforeNavigate.OnNext(args);
        
        if (args.Cancel)
            return Task.CompletedTask;
        
        tcsResults.TryGetValue(viewModel, out var tcs);

        try
        {
            tcs?.TrySetResult(result);
            tcsResults.Remove(viewModel);
            return GoBackAsync(viewModel);
        }
        catch (Exception ex)
        {
            tcs?.TrySetException(ex);
            return Task.FromException(ex);
        }
    }
    
    private async Task<bool> CanNavigate(NavigationMode mode, IMvvmViewModel? viewModel)
    {
        if (routerService.GetDefaultRouter().NavigationStack.LastOrDefault() is not MvvmViewModel view) return true;
        return await view.CanNavigateAsync(mode, viewModel);
    }

    #endregion Methods
}