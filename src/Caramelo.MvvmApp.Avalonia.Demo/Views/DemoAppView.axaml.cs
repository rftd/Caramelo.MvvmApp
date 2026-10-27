using System;
using Caramelo.MvvmApp.Demo.Core.ViewModels;
using ReactiveUI.Reactive;

namespace Caramelo.MvvmApp.Avalonia.Demo.Views;

public partial class DemoAppView : MvvmWindow<DemoAppViewModel>
{
    public DemoAppView()
    {
        InitializeComponent();
        this.WhenActivated((Action<Action<IDisposable>>)(disposeWith =>
        {
            var subscription = ViewModel?.OpenHomeCommand.Execute().Subscribe();
            if (subscription is not null) disposeWith(subscription);
        }));
    }
}