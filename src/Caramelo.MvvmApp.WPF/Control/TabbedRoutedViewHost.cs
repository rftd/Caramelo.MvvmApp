using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI.Reactive;

namespace Caramelo.MvvmApp.WPF;

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

    public static readonly DependencyProperty CanCloseTabsProperty =
        DependencyProperty.Register(
            nameof(CanCloseTabs),
            typeof(bool),
            typeof(TabbedRoutedViewHost),
            new PropertyMetadata(true, OnCanCloseTabsChanged));

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

    public bool CanCloseTabs
    {
        get => (bool)GetValue(CanCloseTabsProperty);
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

    private static void OnCanCloseTabsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TabbedRoutedViewHost host)
        {
            host.RefreshTabHeaders();
        }
    }

    public void CloseTab(TabItem tabItem) => coordinator.CloseTab(tabItem);

    public Task<bool> CloseTabAsync(TabItem tabItem) => coordinator.CloseTabAsync(tabItem);

    private void RefreshTabHeaders()
    {
        foreach (var session in coordinator.Sessions)
        {
            if (session.TabItem.Tag is TabHeaderModel model)
            {
                model.CloseButton.Visibility = (session.CanClose && CanCloseTabs)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }

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

    private (object Header, TabHeaderModel Model) BuildTabHeader(TabItem tabItem, IMvvmViewModel viewModel, bool canClose)
    {
        var root = new DockPanel
        {
            LastChildFill = true,
            VerticalAlignment = VerticalAlignment.Center
        };

        var closeButton = new Button
        {
            Content = "×",
            Padding = new Thickness(4, 0, 4, 0),
            Margin = new Thickness(8, 0, 0, 0),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13,
            FontWeight = FontWeights.Bold,
            Focusable = false,
            Cursor = Cursors.Hand,
            ToolTip = "Fechar",
            Visibility = (canClose && CanCloseTabs) ? Visibility.Visible : Visibility.Collapsed
        };

        var template = new ControlTemplate(typeof(Button));
        var border = new FrameworkElementFactory(typeof(Border));
        border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));

        var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
        presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        presenter.SetValue(ContentPresenter.MarginProperty, new Thickness(2, 0, 2, 0));
        border.AppendChild(presenter);
        template.VisualTree = border;

        var hoverTrigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
        hoverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromArgb(32, 128, 128, 128))));
        template.Triggers.Add(hoverTrigger);

        closeButton.Template = template;

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
            VerticalAlignment = VerticalAlignment.Center
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
            model.CloseButton.Visibility = (canClose && CanCloseTabs)
                ? Visibility.Visible
                : Visibility.Collapsed;
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
