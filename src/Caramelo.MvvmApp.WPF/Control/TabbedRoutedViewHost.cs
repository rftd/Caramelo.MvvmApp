using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI.Reactive;

namespace Caramelo.MvvmApp.WPF;

public class TabbedRoutedViewHost : ContentControl, IActivatableView, ITabbedHostAdapter<TabItem>
{
    #region Fields

    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register(
            nameof(Router),
            typeof(RoutingState),
            typeof(TabbedRoutedViewHost),
            new PropertyMetadata(null, OnRouterChanged));

    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(
            nameof(DefaultContent),
            typeof(object),
            typeof(TabbedRoutedViewHost),
            new PropertyMetadata(null, OnDefaultContentChanged));

    private readonly TabbedHostCoordinator<TabItem> coordinator;
    private TabControl? tabControl;

    #endregion Fields

    #region Constructors

    public TabbedRoutedViewHost()
    {
        coordinator = new TabbedHostCoordinator<TabItem>(
            this,
            () => ViewLocator ?? MvvmApp.Current.Services.GetRequiredService<IViewLocator>(),
            MvvmApp.Current?.Services?.GetService<ILogger<TabbedRoutedViewHost>>());

        this.WhenActivated(disposables =>
        {
            coordinator.AttachRouter(Router);
            disposables(Disposable.Create(() => coordinator.DetachRouter()));
        });
    }

    #endregion Constructors

    #region Properties

    public RoutingState? Router
    {
        get => (RoutingState?)GetValue(RouterProperty);
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

    private static void OnRouterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TabbedRoutedViewHost host)
        {
            host.coordinator.AttachRouter(e.NewValue as RoutingState);
        }
    }

    private static void OnDefaultContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TabbedRoutedViewHost host && host.coordinator.Sessions.Count == 0)
        {
            host.Content = e.NewValue;
        }
    }

    public void CloseTab(TabItem tabItem) => coordinator.CloseTab(tabItem);

    private TabControl GetOrCreateTabControl()
    {
        if (tabControl == null)
        {
            tabControl = new TabControl
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch
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
        if (tabControl != null)
        {
            tabControl.SelectedItem = tabItem;
        }
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
