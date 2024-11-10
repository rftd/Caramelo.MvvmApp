using Caramelo.MvvmApp.Demo.Core.ViewModels;
using Caramelo.MvvmApp.WPF.View;

namespace Caramelo.MvvmApp.WPF.Demo.Views;

public partial class DemoAppView : MvvmWindow<DemoAppViewModel>
{
    public DemoAppView()
    {
        InitializeComponent();
    }
}