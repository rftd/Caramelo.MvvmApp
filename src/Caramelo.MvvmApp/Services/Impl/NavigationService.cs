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
    private readonly ConditionalWeakTable<MvvmViewModel, TaskCompletionSource<object>> tcsResults = new();
    private readonly IRouterService routerService = service.GetRequiredService<IRouterService>();

    #endregion Fields

    #region Properties

    public IObservable<MvvmNavigateEventArgs> BeforeNavigate => beforeNavigate.AsObservable();

    #endregion Properties

    #region Methods

    public async Task GoToAsync<TModel>() where TModel : MvvmViewModel
    {
        var view = service.GetRequiredService<TModel>();
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);

        if (await IsNavigationAllowed(args))
            await routerService.GetDefaultRouter().Navigate.Execute(view).ToTask();
    }

    public async Task<TResult?> GoToAsync<TModel, TResult>()
        where TModel : MvvmResultViewModel<TResult> where TResult : notnull
    {
        var view = service.GetRequiredService<TModel>();
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);
        return await NavigateForResult<TModel, TResult>(view, args);
    }

    public async Task GoToAsync<TModel, TParameter>(TParameter parameter)
        where TModel : MvvmViewModel<TParameter> where TParameter : notnull
    {
        var view = service.GetRequiredService<TModel>();
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);

        if (!await IsNavigationAllowed(args)) return;

        view.Initialize(parameter);
        await routerService.GetDefaultRouter().Navigate.Execute(view).ToTask();
    }

    public async Task<TResult?> GoToAsync<TModel, TParameter, TResult>(TParameter parameter)
        where TModel : MvvmViewModel<TParameter, TResult> where TParameter : notnull where TResult : notnull
    {
        var view = service.GetRequiredService<TModel>();
        var args = new MvvmNavigateEventArgs(view, NavigationMode.Show);
        return await NavigateForResult<TModel, TResult>(view, args, () => view.Initialize(parameter));
    }

    public Task<bool> CanGoBack(IMvvmViewModel? viewModel = null)
    {
        return CanNavigate(NavigationMode.Close, viewModel);
    }

    public Task<bool> CanGoTo(IMvvmViewModel? viewModel = null)
    {
        return CanNavigate(NavigationMode.Show, viewModel);
    }

    public async Task GoBackAsync<TModel>(TModel viewModel) where TModel : MvvmViewModel
    {
        var args = new MvvmNavigateEventArgs(viewModel, NavigationMode.Close);
        if (await IsNavigationAllowed(args))
        {
            var router = routerService.GetDefaultRouter();
            if (router.NavigationStack.LastOrDefault() == viewModel)
                await router.NavigateBack.Execute(Unit.Default).ToTask();
            else if (router.NavigationStack.Contains(viewModel))
                router.NavigationStack.Remove(viewModel);
        }
    }

    public Task GoBackAsync<TParameter, TResult>(MvvmViewModel<TParameter, TResult> viewModel, TResult result)
        where TParameter : notnull where TResult : notnull
    {
        return GoBackWithResultAsync(viewModel, result);
    }

    public Task GoBackAsync<TResult>(MvvmResultViewModel<TResult> viewModel, TResult result) where TResult : notnull
    {
        return GoBackWithResultAsync(viewModel, result);
    }

    private async Task<bool> CanNavigate(NavigationMode mode, IMvvmViewModel? viewModel)
    {
        var targetViewModel = mode == NavigationMode.Close && viewModel is MvvmViewModel closeVm
            ? closeVm
            : routerService.GetDefaultRouter().NavigationStack.LastOrDefault() as MvvmViewModel;

        if (targetViewModel is null) return true;
        return await targetViewModel.CanNavigateAsync(mode, viewModel);
    }

    private async Task<bool> IsNavigationAllowed(MvvmNavigateEventArgs args)
    {
        beforeNavigate.OnNext(args);
        if (args.Cancel) return false;

        var targetViewModel = args.Mode == NavigationMode.Close && args.ViewModel is MvvmViewModel closeVm
            ? closeVm
            : routerService.GetDefaultRouter().NavigationStack.LastOrDefault() as MvvmViewModel;

        if (targetViewModel is null)
            return true;

        if (await targetViewModel.CanNavigateAsync(args.Mode, args.ViewModel)) return !args.Cancel;

        args.Cancel = true;
        return false;
    }

    private async Task<TResult?> NavigateForResult<TViewModel, TResult>(
        TViewModel viewModel,
        MvvmNavigateEventArgs args,
        Action? initialize = null)
        where TViewModel : MvvmResultViewModel<TResult>
        where TResult : notnull
    {
        if (!await IsNavigationAllowed(args)) return default;

        var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        tcsResults.Add(viewModel, completion);

        try
        {
            initialize?.Invoke();
            await routerService.GetDefaultRouter().Navigate.Execute(viewModel).ToTask();
            return (TResult)await completion.Task;
        }
        finally
        {
            tcsResults.Remove(viewModel);
        }
    }

    private async Task GoBackWithResultAsync<TResult>(MvvmResultViewModel<TResult> viewModel, TResult result)
        where TResult : notnull
    {
        var args = new MvvmNavigateEventArgs(viewModel, NavigationMode.Close);
        if (!await IsNavigationAllowed(args)) return;

        TaskCompletionSource<object>? completion = null;
        tcsResults.TryGetValue(viewModel, out completion);

        try
        {
            var router = routerService.GetDefaultRouter();
            if (router.NavigationStack.LastOrDefault() == viewModel)
                await router.NavigateBack.Execute(Unit.Default).ToTask();
            else if (router.NavigationStack.Contains(viewModel))
                router.NavigationStack.Remove(viewModel);

            completion?.TrySetResult(result);
        }
        catch (Exception exception)
        {
            completion?.TrySetException(exception);
            throw;
        }
        finally
        {
            tcsResults.Remove(viewModel);
        }
    }

    #endregion Methods
}