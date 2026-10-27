using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.Services;
using Caramelo.MvvmApp.Services.Impl;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Caramelo.MvvmApp.Tests;

public sealed class NavigationServiceTests
{
    [Fact]
    public async Task GoToAsync_ReturnsTypedResultAfterNavigatingBack()
    {
        await using var services = CreateServices();
        var navigation = services.GetRequiredService<INavigationService>();
        var router = services.GetRequiredService<IRouterService>().GetDefaultRouter();

        var resultTask = navigation.GoToAsync<IntResultViewModel, int>();
        await WaitUntilAsync(() => router.NavigationStack.Count == 1);
        var viewModel = Assert.IsType<IntResultViewModel>(router.NavigationStack.Single());

        await navigation.GoBackAsync(viewModel, 42);

        Assert.Equal(42, await resultTask.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task GoToAsync_WithParameterReturnsTypedResult()
    {
        await using var services = CreateServices();
        var navigation = services.GetRequiredService<INavigationService>();
        var router = services.GetRequiredService<IRouterService>().GetDefaultRouter();

        var resultTask = navigation.GoToAsync<ParameterizedResultViewModel, string, int>("parameter");
        await WaitUntilAsync(() => router.NavigationStack.Count == 1);
        var viewModel = Assert.IsType<ParameterizedResultViewModel>(router.NavigationStack.Single());

        Assert.Equal("parameter", viewModel.Parameter);
        await navigation.GoBackAsync(viewModel, 17);

        Assert.Equal(17, await resultTask.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task GoToAsync_WaitsForCurrentViewModelAsyncNavigationCheck()
    {
        await using var services = CreateServices();
        var navigation = services.GetRequiredService<INavigationService>();
        var router = services.GetRequiredService<IRouterService>().GetDefaultRouter();

        await navigation.GoToAsync<BlockingViewModel>();
        var blocker = Assert.IsType<BlockingViewModel>(router.NavigationStack.Single());
        var targetNavigation = navigation.GoToAsync<IntResultViewModel, int>();

        await blocker.NavigationCheckStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Single(router.NavigationStack);

        blocker.AllowNavigation.TrySetResult(false);
        await targetNavigation.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Single(router.NavigationStack);
        Assert.IsType<BlockingViewModel>(router.NavigationStack.Single());
    }

    [Fact]
    public async Task GoToAsync_PropagatesCurrentViewModelNavigationException()
    {
        await using var services = CreateServices();
        var navigation = services.GetRequiredService<INavigationService>();
        var router = services.GetRequiredService<IRouterService>().GetDefaultRouter();

        await navigation.GoToAsync<ThrowingViewModel>();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            navigation.GoToAsync<IntResultViewModel, int>);

        Assert.Equal("Navigation check failed.", exception.Message);
        Assert.Single(router.NavigationStack);
        Assert.IsType<ThrowingViewModel>(router.NavigationStack.Single());
    }

    private static ServiceProvider CreateServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IRouterService, RouterService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, StubDialogService>();
        services.AddTransient<IntResultViewModel>();
        services.AddTransient<ParameterizedResultViewModel>();
        services.AddTransient<BlockingViewModel>();
        services.AddTransient<ThrowingViewModel>();
        return services.BuildServiceProvider();
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!condition())
            await Task.Delay(10, timeout.Token);
    }
}

public sealed class IntResultViewModel(IServiceProvider service) : MvvmResultViewModel<int>(service);

public sealed class ParameterizedResultViewModel(IServiceProvider service)
    : MvvmViewModel<string, int>(service)
{
    public string? Parameter { get; private set; }

    public override void Initialize(string parameter) => Parameter = parameter;
}

public sealed class BlockingViewModel(IServiceProvider service) : MvvmViewModel(service)
{
    public TaskCompletionSource NavigationCheckStarted { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public TaskCompletionSource<bool> AllowNavigation { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public override async Task<bool> CanNavigateAsync(NavigationMode mode, IMvvmViewModel? viewModel)
    {
        NavigationCheckStarted.TrySetResult();
        return await AllowNavigation.Task;
    }
}

public sealed class ThrowingViewModel(IServiceProvider service) : MvvmViewModel(service)
{
    public override Task<bool> CanNavigateAsync(NavigationMode mode, IMvvmViewModel? viewModel) =>
        Task.FromException<bool>(new InvalidOperationException("Navigation check failed."));
}

internal sealed class StubDialogService : IDialogService
{
    public Task ShowAsync(string title, string message) => Task.CompletedTask;
    public Task InfoAsync(string message) => Task.CompletedTask;
    public Task WarnAsync(string message) => Task.CompletedTask;
    public Task ErroAsync(string message) => Task.CompletedTask;
    public Task<string> InputAsync(string title, string message) => Task.FromResult(string.Empty);
    public Task<bool> ConfirmAsync(string title, string message) => Task.FromResult(false);

    public Task<TResult> ShowAsync<TViewModel, TResult, TParameter>(TParameter parameter)
        where TViewModel : MvvmDialogViewModel<TParameter, TResult>
        where TParameter : DialogOptions
        where TResult : notnull =>
        Task.FromException<TResult>(new NotSupportedException());

    public Task<TResult> ShowAsync<TViewModel, TResult>(DialogOptions options)
        where TViewModel : MvvmDialogViewModel<DialogOptions, TResult>
        where TResult : notnull =>
        Task.FromException<TResult>(new NotSupportedException());

    public Task<TResult> ShowAsync<TViewModel, TResult>()
        where TViewModel : MvvmDialogViewModel<DialogOptions, TResult>
        where TResult : notnull =>
        Task.FromException<TResult>(new NotSupportedException());

    public Task ShowAsync<TViewModel>(DialogOptions options)
        where TViewModel : MvvmDialogViewModel<DialogOptions, System.Reactive.Unit> =>
        Task.FromException(new NotSupportedException());

    public Task ShowAsync<TViewModel>()
        where TViewModel : MvvmDialogViewModel<DialogOptions, System.Reactive.Unit> =>
        Task.FromException(new NotSupportedException());
}
