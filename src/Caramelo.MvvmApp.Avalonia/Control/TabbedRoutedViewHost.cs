using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Splat;

namespace Caramelo.MvvmApp.Avalonia;

public class TabbedRoutedViewHost : ContentControl, IActivatableView, IEnableLogger
{
    #region Fields

    public static readonly StyledProperty<RoutingState?> RouterProperty =
        AvaloniaProperty.Register<RoutedViewHost, RoutingState?>(nameof(Router));

    public static readonly StyledProperty<object?> DefaultContentProperty =
        ViewModelViewHost.DefaultContentProperty.AddOwner<RoutedViewHost>();

    #endregion

    #region Constructors

    public TabbedRoutedViewHost()
    {
        this.WhenActivated(disposables =>
        {
            var routerRemoved = this
                .WhenAnyValue(x => x.Router)
                .Where(router => router == null)!
                .Cast<object?>();

            this.WhenAnyValue(x => x.Router)
                .Where(router => router != null)
                .SelectMany(router => router!.NavigationStack.Events().CollectionChanged)
                .Merge(routerRemoved)
                .Where(x => x is NotifyCollectionChangedEventArgs
                {
                    Action: NotifyCollectionChangedAction.Reset or
                    NotifyCollectionChangedAction.Add or
                    NotifyCollectionChangedAction.Remove
                })
                .Subscribe(x => NavigateToViewModel(x as NotifyCollectionChangedEventArgs))
                .DisposeWith(disposables);
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

    #endregion Properties

    #region Methods

    private void NavigateToViewModel(NotifyCollectionChangedEventArgs? eventArgs)
    {
        if (Router == null)
        {
            this.Log().Warn("Router property is null. Falling back to default content.");
            Content = DefaultContent;
            return;
        }

        var viewModel = (IMvvmViewModel?)(eventArgs?.NewItems?[0] ?? eventArgs?.OldItems?[0]);
        if (viewModel == null || eventArgs == null)
        {
            this.Log().Info("ViewModel is null. Falling back to default content.");
            Content = DefaultContent;
            return;
        }

        switch (eventArgs.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddTab(viewModel);
                break;

            case NotifyCollectionChangedAction.Remove:
                RemoveTab(viewModel);
                break;

            case NotifyCollectionChangedAction.Reset:
                if (Content is TabControl tabControl)
                    tabControl.Items.Clear();

                Content = DefaultContent;
                break;

            default:
                Content = DefaultContent;
                break;
        }
    }

    private void AddTab(IMvvmViewModel viewModel)
    {
        var viewLocator = ViewLocator ?? MvvmApp.Current.Services.GetRequiredService<IViewLocator>();
        var viewInstance = viewLocator.ResolveView(viewModel);
        if (viewInstance == null)
        {
            this.Log().Warn(
                $"Couldn't find view for '{viewModel}'. Is it registered? Falling back to default content.");
            return;
        }

        viewInstance.ViewModel = viewModel;

        var tabControl = Content as TabControl ?? new TabControl
        {
            HorizontalContentAlignment = global::Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalContentAlignment = global::Avalonia.Layout.VerticalAlignment.Stretch
        };
        tabControl.Items.Add(new TabItem
        {
            Header = viewModel.Title,
            Content = viewInstance,
        });

        tabControl.SelectedIndex = tabControl.Items.Count - 1;

        if (Content is not TabControl)
            Content = tabControl;
    }

    private void RemoveTab(IMvvmViewModel viewModel)
    {
        if (Content is not TabControl tabControl) return;
        var tab = tabControl.Items.SingleOrDefault(x => x is TabItem { Content: IViewFor viewFor } &&
                                                        viewFor.ViewModel == viewModel);

        if (tab != null)
        {
            tabControl.Items.Remove(tab);
            if (tabControl.Items.Count == 0)
            {
                Content = DefaultContent;
                tabControl.SelectedIndex = 0;
            }
        }
    }

    #endregion Methods
}