using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Caramelo.MvvmApp.Demo.Core.ViewModels;

namespace Caramelo.MvvmApp.Avalonia.Demo.Views;

public partial class DemoView : MvvmUserControl<DemoViewModel>
{
    public DemoView()
    {
        InitializeComponent();
    }
}