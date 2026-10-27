using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.Reactive;

namespace Caramelo.MvvmApp.Navigation;

public sealed class TabHistoryEntry : IDisposable
{
    public required IMvvmViewModel ViewModel { get; init; }
    
    public required IViewFor ViewInstance { get; init; }
    
    public bool CanClose { get; init; } = true;
    
    public IDisposable? TitleSubscription { get; set; }

    public void Dispose()
    {
        TitleSubscription?.Dispose();
        TitleSubscription = null;
    }
}

public sealed class TabSession<TTabItem>(TTabItem tabItem)
    where TTabItem : class
{
    public TTabItem TabItem { get; } = tabItem;
    
    public bool CanClose { get; set; } = true;
    
    public List<TabHistoryEntry> History { get; } = [];
}

public class TabbedHostCoordinator<TTabItem>(
    ITabbedHostAdapter<TTabItem> adapter,
    Func<IViewLocator> viewLocatorResolver,
    ILogger? logger = null)
    : IDisposable
    where TTabItem : class
{
    #region Fields

    private readonly ITabbedHostAdapter<TTabItem> adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
    
    private readonly Func<IViewLocator> viewLocatorResolver = viewLocatorResolver ?? throw new ArgumentNullException(nameof(viewLocatorResolver));

    private readonly List<TabSession<TTabItem>> sessions = [];
    
    private RoutingState? router;
    
    private IDisposable? routerSubscription;

    #endregion Fields

    #region Properties

    public IReadOnlyList<TabSession<TTabItem>> Sessions => sessions;

    public RoutingState? Router => router;

    #endregion Properties

    #region Methods

    public void AttachRouter(RoutingState? newRouter)
    {
        if (ReferenceEquals(router, newRouter)) return;

        routerSubscription?.Dispose();
        routerSubscription = null;
        router = newRouter;
        Reset();

        if (router == null) return;

        foreach (var item in router.NavigationStack)
        {
            if (item is IMvvmViewModel vm)
                AddTab(vm);
        }

        routerSubscription = Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            handler => (sender, args) => handler(args),
            h => router.NavigationStack.CollectionChanged += h,
            h => router.NavigationStack.CollectionChanged -= h)
            .Subscribe(NavigateToViewModel);
    }

    public void DetachRouter()
    {
        routerSubscription?.Dispose();
        routerSubscription = null;
        router = null;
        Reset();
    }

    public void NavigateToViewModel(NotifyCollectionChangedEventArgs eventArgs)
    {
        if (eventArgs == null) return;

        switch (eventArgs.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (eventArgs.NewItems != null)
                {
                    foreach (var item in eventArgs.NewItems)
                    {
                        if (item is IMvvmViewModel vm)
                            AddTab(vm);
                    }
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                if (eventArgs.OldItems != null)
                {
                    foreach (var item in eventArgs.OldItems)
                    {
                        if (item is IMvvmViewModel vm)
                            RemoveTab(vm);
                    }
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                Reset();
                break;
        }
    }

    public void AddTab(IMvvmViewModel viewModel)
    {
        var locator = viewLocatorResolver();
        var viewInstance = locator.ResolveViewUnsafe(viewModel, contract: null);
        if (viewInstance == null)
        {
            logger?.LogWarning("Couldn't find view for '{ViewModel}'. Falling back to default content.", viewModel);
            if (sessions.Count == 0)
                adapter.ShowDefaultContent();
            return;
        }

        viewInstance.ViewModel = viewModel;
        viewModel.ViewCreated();

        var mode = TabNavigationResolver.ResolveMode(viewInstance.GetType(), viewModel.GetType());
        var canClose = adapter.CanCloseTabs && TabNavigationResolver.ResolveCanClose(viewInstance.GetType(), viewModel.GetType(), defaultCanClose: true);

        var entry = new TabHistoryEntry
        {
            ViewModel = viewModel,
            ViewInstance = viewInstance,
            CanClose = canClose
        };

        if (mode == TabNavigationMode.CurrentTab && sessions.Count > 0)
        {
            var selectedTabItem = adapter.GetSelectedTabItem();
            var currentSession = sessions.FirstOrDefault(s => ReferenceEquals(s.TabItem, selectedTabItem)) ?? sessions[^1];

            currentSession.CanClose = canClose;
            currentSession.History.Add(entry);
            SubscribeToTitle(entry, currentSession);

            adapter.UpdateTabItem(currentSession.TabItem, viewModel, viewInstance, canClose);
            if (!ReferenceEquals(adapter.GetSelectedTabItem(), currentSession.TabItem))
            {
                adapter.SetSelectedTabItem(currentSession.TabItem);
            }
        }
        else
        {
            var tabItem = adapter.CreateTabItem(viewModel, viewInstance, canClose);
            var session = new TabSession<TTabItem>(tabItem)
            {
                CanClose = canClose
            };
            session.History.Add(entry);
            SubscribeToTitle(entry, session);

            sessions.Add(session);
            adapter.AddTabItem(tabItem);
            adapter.SetSelectedTabItem(tabItem);
            adapter.ShowTabs();
        }
    }

    public void RemoveTab(IMvvmViewModel viewModel)
    {
        var session = sessions.FirstOrDefault(s => s.History.Any(e => ReferenceEquals(e.ViewModel, viewModel)));
        if (session == null) return;

        var entry = session.History.FirstOrDefault(e => ReferenceEquals(e.ViewModel, viewModel));
        if (entry != null)
        {
            entry.Dispose();
            session.History.Remove(entry);
        }

        if (session.History.Count > 0)
        {
            var previous = session.History[^1];
            session.CanClose = previous.CanClose;
            adapter.UpdateTabItem(session.TabItem, previous.ViewModel, previous.ViewInstance, previous.CanClose);
            adapter.SetSelectedTabItem(session.TabItem);
        }
        else
        {
            sessions.Remove(session);
            adapter.RemoveTabItem(session.TabItem);

            if (sessions.Count == 0)
            {
                adapter.ShowDefaultContent();
            }
            else
            {
                var selected = adapter.GetSelectedTabItem();
                if (selected == null || ReferenceEquals(selected, session.TabItem))
                {
                    adapter.SetSelectedTabItem(sessions[^1].TabItem);
                }
            }
        }
    }

    public async Task<bool> CloseTabAsync(TTabItem tabItem)
    {
        var session = sessions.FirstOrDefault(s => ReferenceEquals(s.TabItem, tabItem));
        if (session == null) return false;

        var viewModels = session.History.Select(e => e.ViewModel).Reverse().ToList();
        foreach (var vm in viewModels)
        {
            if (vm is MvvmViewModel mvvm)
            {
                var allowed = await mvvm.CanNavigateAsync(NavigationMode.Close, vm);
                if (!allowed)
                    return false;
            }
        }

        CloseTabInternal(session);
        return true;
    }

    public void CloseTab(TTabItem tabItem)
    {
        var session = sessions.FirstOrDefault(s => ReferenceEquals(s.TabItem, tabItem));
        if (session == null) return;

        CloseTabInternal(session);
    }

    private void CloseTabInternal(TabSession<TTabItem> session)
    {
        var viewModels = session.History.Select(e => e.ViewModel).Reverse().ToList();
        foreach (var vm in viewModels)
        {
            if (vm is IRoutableViewModel routable && router?.NavigationStack.Contains(routable) == true)
            {
                router.NavigationStack.Remove(routable);
            }
            else
            {
                RemoveTab(vm);
            }
        }
    }

    public void Reset()
    {
        foreach (var entry in sessions.SelectMany(session => session.History))
        {
            entry.Dispose();
        }
        sessions.Clear();
        adapter.ClearTabItems();
        adapter.ShowDefaultContent();
    }

    private void SubscribeToTitle(TabHistoryEntry entry, TabSession<TTabItem> session)
    {
        if (entry.ViewModel is not INotifyPropertyChanged inpc) return;
        PropertyChangedEventHandler handler = (_, e) =>
        {
            if (e.PropertyName != nameof(IMvvmViewModel.Title) && !string.IsNullOrEmpty(e.PropertyName)) return;
            if (session.History.LastOrDefault() == entry)
            {
                adapter.UpdateTabItem(session.TabItem, entry.ViewModel, entry.ViewInstance, entry.CanClose);
            }
        };
        inpc.PropertyChanged += handler;
        entry.TitleSubscription = Disposable.Create(() => inpc.PropertyChanged -= handler);
    }

    public void Dispose()
    {
        routerSubscription?.Dispose();
        routerSubscription = null;
        Reset();
    }

    #endregion Methods
}
