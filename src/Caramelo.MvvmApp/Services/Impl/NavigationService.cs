using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Caramelo.MvvmApp.Extensions;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using ReactiveUI;

namespace Caramelo.MvvmApp.Services.Impl;

internal class NavigationService : INavigationService
{
    #region Fields

    private Subject<MvvmNavigateEventArgs> beforeNavigate;

    #endregion Fields
    
    #region Fields

    private readonly ConditionalWeakTable<MvvmViewModel, TaskCompletionSource<object>?> tcsResults;
    private readonly IServiceProvider service;

    #endregion Fields
    
    #region Constructors

    public NavigationService(IServiceProvider service)
    {
        this.service = service;
        Router = new RoutingState();
        tcsResults = new ConditionalWeakTable<MvvmViewModel, TaskCompletionSource<object>?>();
        beforeNavigate = new Subject<MvvmNavigateEventArgs>();
    }

    #endregion Constructors

    #region Properties

    internal RoutingState Router { get; }

    public IObservable<MvvmNavigateEventArgs> BeforeNavigate => beforeNavigate.AsObservable();

    #endregion Properties

    #region Methods

    public Task GoToAsync<TModel>() where TModel : MvvmViewModel
    {
        var view = service.GetRequiredService<TModel>();
        
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);
        beforeNavigate.OnNext(args);

        return args.Cancel ? Task.CompletedTask : Router.Navigate.Execute(view).ToTask();
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
        
        await Router.Navigate.Execute(view).ToTask();

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
        return Router.Navigate.Execute(view).ToTask();
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
        
        await Router.Navigate.Execute(view).ToTask();

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
        
        return args.Cancel ? Task.CompletedTask : Router.NavigateBack.Execute(Unit.Default).ToTask();
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
        if (Router.NavigationStack.LastOrDefault() is not MvvmViewModel view) return true;
        return await view.CanNavigateAsync(mode, viewModel);
    }

    #endregion Methods
}