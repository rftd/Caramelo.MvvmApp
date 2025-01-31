using Caramelo.MvvmApp.Demo.Core.ViewModels;

namespace Caramelo.MvvmApp.WPF.Demo.Views;

public partial class DemoAppView : MvvmWindow<DemoAppViewModel>
{
    public DemoAppView()
    {
        InitializeComponent();
    }
}