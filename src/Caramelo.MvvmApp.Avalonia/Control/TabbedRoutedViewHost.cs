using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI.Avalonia.Reactive;
using ReactiveUI.Reactive;

namespace Caramelo.MvvmApp.Avalonia;

public class TabbedRoutedViewHost : ContentControl, IActivatableView, ITabbedHostAdapter<TabItem>
{
    #region Fields

    public static readonly StyledProperty<RoutingState?> RouterProperty =
        AvaloniaProperty.Register<TabbedRoutedViewHost, RoutingState?>(nameof(Router));

    public static readonly StyledProperty<object?> DefaultContentProperty =
        AvaloniaProperty.Register<TabbedRoutedViewHost, object?>(nameof(DefaultContent));

    private readonly TabbedHostCoordinator<TabItem> coordinator;
    private TabControl? tabControl;

    #endregion Fields

    #region Constructors

    static TabbedRoutedViewHost()
    {
        DefaultContentProperty.Changed.AddClassHandler<TabbedRoutedViewHost>((host, _) =>
        {
            if (host.coordinator.Sessions.Count == 0)
                host.Content = host.DefaultContent;
        });
    }

    public TabbedRoutedViewHost()
    {
        coordinator = new TabbedHostCoordinator<TabItem>(
            this,
            () => ViewLocator ?? MvvmApp.Current.Services.GetRequiredService<IViewLocator>(),
            MvvmApp.Current?.Services?.GetService<ILogger<TabbedRoutedViewHost>>());

        this.WhenActivated(disposables =>
        {
            var routerSubscription = this.WhenAnyValue(x => x.Router)
                .Subscribe(router => coordinator.AttachRouter(router));

            disposables(routerSubscription);
            disposables(Disposable.Create(() => coordinator.DetachRouter()));
        });
    }

    #endregion Constructors

    #region Properties

    public RoutingState? Router
    {
        get => GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    public object? DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    public IViewLocator? ViewLocator { get; set; }

    public IReadOnlyList<TabSession<TabItem>> Tabs => coordinator.Sessions;

    #endregion Properties

    #region Methods

    public void CloseTab(TabItem tabItem) => coordinator.CloseTab(tabItem);

    private TabControl GetOrCreateTabControl()
    {
        if (tabControl == null)
        {
            tabControl = new TabControl
            {
                HorizontalContentAlignment = global::Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalContentAlignment = global::Avalonia.Layout.VerticalAlignment.Stretch
            };
        }
        return tabControl;
    }

    TabItem ITabbedHostAdapter<TabItem>.CreateTabItem(IMvvmViewModel viewModel, IViewFor viewInstance)
    {
        return new TabItem
        {
            Header = viewModel.Title,
            Content = viewInstance
        };
    }

    void ITabbedHostAdapter<TabItem>.UpdateTabItem(TabItem tabItem, IMvvmViewModel viewModel, IViewFor viewInstance)
    {
        tabItem.Header = viewModel.Title;
        tabItem.Content = viewInstance;
    }

    void ITabbedHostAdapter<TabItem>.AddTabItem(TabItem tabItem)
    {
        var control = GetOrCreateTabControl();
        control.Items.Add(tabItem);
    }

    void ITabbedHostAdapter<TabItem>.RemoveTabItem(TabItem tabItem)
    {
        tabControl?.Items.Remove(tabItem);
    }

    void ITabbedHostAdapter<TabItem>.ClearTabItems()
    {
        tabControl?.Items.Clear();
    }

    TabItem? ITabbedHostAdapter<TabItem>.GetSelectedTabItem()
    {
        return tabControl?.SelectedItem as TabItem;
    }

    void ITabbedHostAdapter<TabItem>.SetSelectedTabItem(TabItem? tabItem)
    {
        tabControl?.SelectedItem = tabItem;
    }

    void ITabbedHostAdapter<TabItem>.ShowTabs()
    {
        var control = GetOrCreateTabControl();
        if (!ReferenceEquals(Content, control))
        {
            Content = control;
        }
    }

    void ITabbedHostAdapter<TabItem>.ShowDefaultContent()
    {
        Content = DefaultContent;
    }

    #endregion Methods
}