using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using ReactiveUI.Reactive;
using Xunit;

namespace Caramelo.MvvmApp.Tests;

public sealed class TabbedHostCoordinatorTests
{
    [Fact]
    public void TabNavigationResolver_ResolvesCorrectMode()
    {
        // 1. Default (no attributes on either) -> CurrentTab
        Assert.Equal(TabNavigationMode.CurrentTab,
            TabNavigationResolver.ResolveMode(typeof(DefaultView), typeof(DefaultViewModel)));

        // 2. ViewModel has NewTab, View has no attribute -> NewTab
        Assert.Equal(TabNavigationMode.NewTab,
            TabNavigationResolver.ResolveMode(typeof(DefaultView), typeof(NewTabViewModel)));

        // 3. View has NewTab, ViewModel has no attribute -> NewTab
        Assert.Equal(TabNavigationMode.NewTab,
            TabNavigationResolver.ResolveMode(typeof(NewTabView), typeof(DefaultViewModel)));

        // 4. View has CurrentTab, ViewModel has NewTab -> View takes priority (CurrentTab)
        Assert.Equal(TabNavigationMode.CurrentTab,
            TabNavigationResolver.ResolveMode(typeof(CurrentTabView), typeof(NewTabViewModel)));

        // 5. View has NewTab, ViewModel has CurrentTab -> View takes priority (NewTab)
        Assert.Equal(TabNavigationMode.NewTab,
            TabNavigationResolver.ResolveMode(typeof(NewTabView), typeof(CurrentTabViewModel)));
    }

    [Fact]
    public void FirstNavigation_AlwaysOpensInNewTab_EvenIfCurrentTabMode()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        var vm = new StubViewModel("First");
        coordinator.AddTab(vm);

        Assert.Single(adapter.Tabs);
        Assert.Equal("First", adapter.Tabs[0].Header);
        Assert.Same(adapter.Tabs[0], adapter.SelectedTab);
        Assert.True(adapter.ShowingTabs);
    }

    [Fact]
    public void SecondNavigation_WithCurrentTabMode_OpensInCurrentTab_AndPushesHistory()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        var vm1 = new StubViewModel("Page 1");
        coordinator.AddTab(vm1);

        var vm2 = new StubViewModel("Page 2");
        coordinator.AddTab(vm2);

        // Still only 1 tab open
        Assert.Single(adapter.Tabs);
        Assert.Equal("Page 2", adapter.Tabs[0].Header);
        Assert.Same(adapter.Tabs[0], adapter.SelectedTab);

        // Tab has 2 history entries
        var session = Assert.Single(coordinator.Sessions);
        Assert.Equal(2, session.History.Count);
        Assert.Same(vm1, session.History[0].ViewModel);
        Assert.Same(vm2, session.History[1].ViewModel);
    }

    [Fact]
    public void SecondNavigation_WithNewTabMode_OpensInNewTab()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        var vm1 = new StubViewModel("Tab 1");
        coordinator.AddTab(vm1);

        var vm2 = new StubNewTabViewModel("Tab 2");
        coordinator.AddTab(vm2);

        Assert.Equal(2, adapter.Tabs.Count);
        Assert.Equal("Tab 1", adapter.Tabs[0].Header);
        Assert.Equal("Tab 2", adapter.Tabs[1].Header);
        Assert.Same(adapter.Tabs[1], adapter.SelectedTab);
        Assert.Equal(2, coordinator.Sessions.Count);
    }

    [Fact]
    public void BackNavigation_InCurrentTab_RestoresPreviousViewInThatTab()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        var vm1 = new StubViewModel("Page 1");
        coordinator.AddTab(vm1);

        var vm2 = new StubViewModel("Page 2");
        coordinator.AddTab(vm2);

        // Go back from Page 2
        coordinator.RemoveTab(vm2);

        // Tab remains open, showing Page 1
        Assert.Single(adapter.Tabs);
        Assert.Equal("Page 1", adapter.Tabs[0].Header);
        Assert.Same(adapter.Tabs[0], adapter.SelectedTab);

        var session = Assert.Single(coordinator.Sessions);
        Assert.Single(session.History);
        Assert.Same(vm1, session.History[0].ViewModel);
    }

    [Fact]
    public void BackNavigation_WhenTabHasSingleView_ClosesTab()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        var vm1 = new StubViewModel("Tab 1");
        coordinator.AddTab(vm1);

        var vm2 = new StubNewTabViewModel("Tab 2");
        coordinator.AddTab(vm2);

        Assert.Equal(2, adapter.Tabs.Count);

        // Remove Tab 2's view
        coordinator.RemoveTab(vm2);

        Assert.Single(adapter.Tabs);
        Assert.Equal("Tab 1", adapter.Tabs[0].Header);
        Assert.Same(adapter.Tabs[0], adapter.SelectedTab);

        // Remove Tab 1's view
        coordinator.RemoveTab(vm1);

        Assert.Empty(adapter.Tabs);
        Assert.False(adapter.ShowingTabs);
    }

    [Fact]
    public void NavigationToCurrentTab_TargetsTheCurrentlySelectedTab()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        var tab1Vm = new StubViewModel("Tab 1");
        coordinator.AddTab(tab1Vm);

        var tab2Vm = new StubNewTabViewModel("Tab 2");
        coordinator.AddTab(tab2Vm);

        // User switches back to Tab 1
        adapter.SetSelectedTabItem(adapter.Tabs[0]);

        // Navigate with CurrentTab mode
        var tab1SubVm = new StubViewModel("Tab 1 SubPage");
        coordinator.AddTab(tab1SubVm);

        // Tab 1 now shows SubPage, Tab 2 is unchanged
        Assert.Equal(2, adapter.Tabs.Count);
        Assert.Equal("Tab 1 SubPage", adapter.Tabs[0].Header);
        Assert.Equal("Tab 2", adapter.Tabs[1].Header);

        // History of Tab 1 has 2 entries; Tab 2 has 1 entry
        Assert.Equal(2, coordinator.Sessions[0].History.Count);
        Assert.Single(coordinator.Sessions[1].History);
    }

    [Fact]
    public void CloseTab_RemovesAllViewModelsFromRouterAndClosesTab()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        var router = new RoutingState();
        coordinator.AttachRouter(router);

        var tab1Vm = new StubViewModel("Tab 1");
        var tab1ChildVm = new StubViewModel("Tab 1 Child");
        router.NavigationStack.Add(tab1Vm);
        router.NavigationStack.Add(tab1ChildVm);

        var tab2Vm = new StubNewTabViewModel("Tab 2");
        router.NavigationStack.Add(tab2Vm);

        Assert.Equal(2, adapter.Tabs.Count);
        Assert.Equal(3, router.NavigationStack.Count);

        // Close Tab 1 (which has 2 view models)
        coordinator.CloseTab(adapter.Tabs[0]);

        // Tab 1 is closed, only Tab 2 remains
        Assert.Single(adapter.Tabs);
        Assert.Equal("Tab 2", adapter.Tabs[0].Header);
        Assert.Single(router.NavigationStack);
        Assert.Same(tab2Vm, router.NavigationStack[0]);
    }

    [Fact]
    public void ViewModelTitleChange_UpdatesTabHeaderWhenActive()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        var vm = new StubViewModel("Original Title");
        coordinator.AddTab(vm);

        Assert.Equal("Original Title", adapter.Tabs[0].Header);

        vm.UpdateTitle("Updated Title");

        Assert.Equal("Updated Title", adapter.Tabs[0].Header);
    }

    [Fact]
    public void Reset_ClearsAllTabsAndShowsDefaultContent()
    {
        var adapter = new MockTabbedHostAdapter();
        using var coordinator = CreateCoordinator(adapter);

        coordinator.AddTab(new StubViewModel("Tab 1"));
        coordinator.AddTab(new StubNewTabViewModel("Tab 2"));

        Assert.Equal(2, adapter.Tabs.Count);

        coordinator.Reset();

        Assert.Empty(adapter.Tabs);
        Assert.False(adapter.ShowingTabs);
    }

    private static TabbedHostCoordinator<MockTabItem> CreateCoordinator(MockTabbedHostAdapter adapter)
    {
        return new TabbedHostCoordinator<MockTabItem>(
            adapter,
            () => new StubViewLocator());
    }
}

public sealed class MockTabItem
{
    public string? Header { get; set; }
    public object? Content { get; set; }
}

public sealed class MockTabbedHostAdapter : ITabbedHostAdapter<MockTabItem>
{
    public List<MockTabItem> Tabs { get; } = [];
    public MockTabItem? SelectedTab { get; private set; }
    public bool ShowingTabs { get; private set; }

    public MockTabItem CreateTabItem(IMvvmViewModel viewModel, IViewFor viewInstance)
    {
        return new MockTabItem
        {
            Header = viewModel.Title,
            Content = viewInstance
        };
    }

    public void UpdateTabItem(MockTabItem tabItem, IMvvmViewModel viewModel, IViewFor viewInstance)
    {
        tabItem.Header = viewModel.Title;
        tabItem.Content = viewInstance;
    }

    public void AddTabItem(MockTabItem tabItem) => Tabs.Add(tabItem);

    public void RemoveTabItem(MockTabItem tabItem)
    {
        Tabs.Remove(tabItem);
        if (ReferenceEquals(SelectedTab, tabItem))
            SelectedTab = Tabs.LastOrDefault();
    }

    public void ClearTabItems()
    {
        Tabs.Clear();
        SelectedTab = null;
    }

    public MockTabItem? GetSelectedTabItem() => SelectedTab;

    public void SetSelectedTabItem(MockTabItem? tabItem) => SelectedTab = tabItem;

    public void ShowTabs() => ShowingTabs = true;

    public void ShowDefaultContent() => ShowingTabs = false;
}

public class DefaultView : IViewFor<DefaultViewModel>
{
    public DefaultViewModel? ViewModel { get; set; }
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as DefaultViewModel;
    }
}

public class DefaultViewModel : StubViewModel
{
    public DefaultViewModel() : base("Default") { }
}

[NewTab]
public class NewTabView : IViewFor<DefaultViewModel>
{
    public DefaultViewModel? ViewModel { get; set; }
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as DefaultViewModel;
    }
}

[CurrentTab]
public class CurrentTabView : IViewFor<NewTabViewModel>
{
    public NewTabViewModel? ViewModel { get; set; }
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as NewTabViewModel;
    }
}

[NewTab]
public class NewTabViewModel : StubViewModel
{
    public NewTabViewModel() : base("New Tab") { }
}

[CurrentTab]
public class CurrentTabViewModel : StubViewModel
{
    public CurrentTabViewModel() : base("Current Tab") { }
}

public class StubViewModel : ReactiveObject, IMvvmViewModel, IRoutableViewModel
{
    private string title;

    public StubViewModel(string title)
    {
        this.title = title;
    }

    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    public void UpdateTitle(string newTitle) => Title = newTitle;

    public bool IsBusy => false;
    public IServiceProvider Service => null!;
    public string? UrlPathSegment => Title;
    public IScreen HostScreen { get; set; } = null!;

    public void ViewCreated() { }
    public void ViewAppearing() { }
    public void ViewAppeared() { }
    public void ViewDisappearing() { }
    public void ViewDisappeared() { }
    public void ViewDestroy(bool viewFinishing = true) { }
}

[NewTab]
public class StubNewTabViewModel : StubViewModel
{
    public StubNewTabViewModel(string title) : base(title) { }
}

public class StubViewFor : IViewFor
{
    public object? ViewModel { get; set; }
}

public class StubViewLocator : IViewLocator
{
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) where T : class => ResolveViewUnsafe(viewModel, contract);

    public IViewFor? ResolveView(object? viewModel, string? contract = null) => ResolveViewUnsafe(viewModel, contract);

    public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract = null)
    {
        if (viewModel is DefaultViewModel)
            return new DefaultView { ViewModel = (DefaultViewModel)viewModel };

        if (viewModel is NewTabViewModel)
            return new CurrentTabView { ViewModel = (NewTabViewModel)viewModel };

        return new StubViewFor { ViewModel = viewModel };
    }
}
