using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI.Reactive;

namespace Caramelo.MvvmApp.Avalonia;

public class TabbedRoutedViewHost : ContentControl, IActivatableView, ITabbedHostAdapter<TabItem>
{
    #region Nested Types

    private sealed class TabHeaderModel
    {
        public required TextBlock TitleBlock { get; init; }
        
        public required Button CloseButton { get; init; }
        
        public required DockPanel Root { get; init; }
    }

    #endregion Nested Types

    #region Fields

    public static readonly StyledProperty<RoutingState?> RouterProperty =
        AvaloniaProperty.Register<TabbedRoutedViewHost, RoutingState?>(nameof(Router));

    public static readonly StyledProperty<object?> DefaultContentProperty =
        AvaloniaProperty.Register<TabbedRoutedViewHost, object?>(nameof(DefaultContent));

    public static readonly StyledProperty<bool> CanCloseTabsProperty =
        AvaloniaProperty.Register<TabbedRoutedViewHost, bool>(nameof(CanCloseTabs), defaultValue: true);

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

        CanCloseTabsProperty.Changed.AddClassHandler<TabbedRoutedViewHost>((host, _) =>
        {
            host.RefreshTabHeaders();
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

    public bool CanCloseTabs
    {
        get => GetValue(CanCloseTabsProperty);
        set => SetValue(CanCloseTabsProperty, value);
    }

    public bool ShowCloseButton
    {
        get => CanCloseTabs;
        set => CanCloseTabs = value;
    }

    public IViewLocator? ViewLocator { get; set; }

    public IReadOnlyList<TabSession<TabItem>> Tabs => coordinator.Sessions;

    bool ITabbedHostAdapter<TabItem>.CanCloseTabs => CanCloseTabs;

    #endregion Properties

    #region Methods

    public void CloseTab(TabItem tabItem) => coordinator.CloseTab(tabItem);

    public Task<bool> CloseTabAsync(TabItem tabItem) => coordinator.CloseTabAsync(tabItem);

    private void RefreshTabHeaders()
    {
        foreach (var session in coordinator.Sessions)
        {
            if (session.TabItem.Tag is TabHeaderModel model)
            {
                model.CloseButton.IsVisible = session.CanClose && CanCloseTabs;
            }
        }
    }

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

    private (object Header, TabHeaderModel Model) BuildTabHeader(TabItem tabItem, IMvvmViewModel viewModel, bool canClose)
    {
        var root = new DockPanel
        {
            LastChildFill = true,
            VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center
        };

        var closeButton = new Button
        {
            Content = "✕",
            Padding = new Thickness(4, 1),
            Margin = new Thickness(8, 0, 0, 0),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(4),
            VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
            FontSize = 10,
            IsVisible = canClose && CanCloseTabs,
            Cursor = new Cursor(StandardCursorType.Hand)
        };

        ToolTip.SetTip(closeButton, "Fechar");

        closeButton.Click += async (s, e) =>
        {
            e.Handled = true;
            await CloseTabAsync(tabItem);
        };

        DockPanel.SetDock(closeButton, Dock.Right);
        root.Children.Add(closeButton);

        var titleBlock = new TextBlock
        {
            Text = viewModel.Title,
            VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center
        };
        DockPanel.SetDock(titleBlock, Dock.Left);
        root.Children.Add(titleBlock);

        var model = new TabHeaderModel
        {
            TitleBlock = titleBlock,
            CloseButton = closeButton,
            Root = root
        };

        return (root, model);
    }

    TabItem ITabbedHostAdapter<TabItem>.CreateTabItem(IMvvmViewModel viewModel, IViewFor viewInstance, bool canClose)
    {
        var tabItem = new TabItem
        {
            Content = viewInstance
        };

        var (header, model) = BuildTabHeader(tabItem, viewModel, canClose);
        tabItem.Header = header;
        tabItem.Tag = model;

        return tabItem;
    }

    void ITabbedHostAdapter<TabItem>.UpdateTabItem(TabItem tabItem, IMvvmViewModel viewModel, IViewFor viewInstance, bool canClose)
    {
        if (tabItem.Tag is TabHeaderModel model)
        {
            model.TitleBlock.Text = viewModel.Title;
            model.CloseButton.IsVisible = canClose && CanCloseTabs;
        }
        else
        {
            tabItem.Header = viewModel.Title;
        }
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